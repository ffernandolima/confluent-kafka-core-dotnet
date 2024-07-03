using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class RegisteredSchemaBuilder :
        FunctionalBuilder<RegisteredSchema, RegisteredSchemaBuilder>,
        IRegisteredSchemaBuilder
    {
        private enum RegisteredSchemaParameter
        {
            Subject,
            Version,
            Id,
            SchemaString,
            SchemaType,
            SchemaReferences
        }

        private static readonly IDictionary<RegisteredSchemaParameter, Type> DefaultValueMappings =
            new Dictionary<RegisteredSchemaParameter, Type>()
        {
            { RegisteredSchemaParameter.Subject,          typeof(string)                },
            { RegisteredSchemaParameter.Version,          typeof(int)                   },
            { RegisteredSchemaParameter.Id,               typeof(int)                   },
            { RegisteredSchemaParameter.SchemaString,     typeof(string)                },
            { RegisteredSchemaParameter.SchemaType,       typeof(SchemaType)            },
            { RegisteredSchemaParameter.SchemaReferences, typeof(List<SchemaReference>) }
        }
#if NET8_0_OR_GREATER
        .ToFrozenDictionary();
#else
        ;
#endif
        public RegisteredSchemaBuilder(IConfiguration configuration = null)
            : base(seedSubject: null, configuration)
        {
            EnumerateParameters(parameter =>
            {
                if (parameter == RegisteredSchemaParameter.SchemaReferences)
                {
                    AppendParameter(parameters => parameters[(int)parameter] = new List<SchemaReference>());
                }
                else
                {
                    AppendParameter(parameters =>
                    {
                        parameters[(int)parameter] = DefaultValueMappings[parameter].GetDefaultValue();
                    });
                }
            });
        }

        public IRegisteredSchemaBuilder FromConfiguration(string sectionKey)
        {
            if (!string.IsNullOrWhiteSpace(sectionKey))
            {
                var section = GetSection(sectionKey);

                EnumerateParameters(parameter => parameter != RegisteredSchemaParameter.SchemaReferences, parameter =>
                {
                    AppendParameter(parameters =>
                    {
                        parameters[(int)parameter] = section.GetValue(DefaultValueMappings[parameter], parameter.ToString());
                    });
                });
            }
            return this;
        }

        public IRegisteredSchemaBuilder WithSubject(string subject)
        {
            AppendParameter(parameters => parameters[(int)RegisteredSchemaParameter.Subject] = subject);
            return this;
        }

        public IRegisteredSchemaBuilder WithVersion(int version)
        {
            AppendParameter(parameters => parameters[(int)RegisteredSchemaParameter.Version] = version);
            return this;
        }

        public IRegisteredSchemaBuilder WithId(int id)
        {
            AppendParameter(parameters => parameters[(int)RegisteredSchemaParameter.Id] = id);
            return this;
        }

        public IRegisteredSchemaBuilder WithSchemaString(string schemaString)
        {
            AppendParameter(parameters => parameters[(int)RegisteredSchemaParameter.SchemaString] = schemaString);
            return this;
        }

        public IRegisteredSchemaBuilder WithSchemaType(SchemaType schemaType)
        {
            AppendParameter(parameters => parameters[(int)RegisteredSchemaParameter.SchemaType] = schemaType);
            return this;
        }

        public IRegisteredSchemaBuilder WithSchemaReferences(List<SchemaReference> schemaReferences)
        {
            AppendParameter(parameters => parameters[(int)RegisteredSchemaParameter.SchemaReferences] = schemaReferences ?? []);
            return this;
        }

        public static RegisteredSchema Build(IConfiguration configuration, Action<IRegisteredSchemaBuilder> configureRegisteredSchema)
        {
            using var builder = new RegisteredSchemaBuilder(configuration);

            configureRegisteredSchema?.Invoke(builder);

            var registeredSchema = builder.Build();

            return registeredSchema;
        }

        private static void EnumerateParameters(Action<RegisteredSchemaParameter> action) => EnumerateParameters(condition: null, action);

        private static void EnumerateParameters(Func<RegisteredSchemaParameter, bool> condition, Action<RegisteredSchemaParameter> action)
        {
            foreach (var parameter in Enum.GetValues(typeof(RegisteredSchemaParameter))
                .Cast<RegisteredSchemaParameter>())
            {
                if (condition is null || condition.Invoke(parameter))
                {
                    action?.Invoke(parameter);
                }
            }
        }
    }
}
