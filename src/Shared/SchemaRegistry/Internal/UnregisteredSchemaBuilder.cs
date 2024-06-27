using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Serialization.SchemaRegistry.Internal
{
    internal sealed class UnregisteredSchemaBuilder :
        FunctionalBuilder<Schema, UnregisteredSchemaBuilder>,
        IUnregisteredSchemaBuilder
    {
        private enum UnregisteredSchemaParameter
        {
            SchemaString,
            SchemaReferences,
            SchemaType
        }

        private static readonly Dictionary<UnregisteredSchemaParameter, Type> DefaultValueMappings = new()
        {
            { UnregisteredSchemaParameter.SchemaString,     typeof(string)                },
            { UnregisteredSchemaParameter.SchemaType,       typeof(SchemaType)            },
            { UnregisteredSchemaParameter.SchemaReferences, typeof(List<SchemaReference>) }
        };

        public UnregisteredSchemaBuilder(IConfiguration configuration = null)
           : base(seedSubject: null, configuration)
        {
            foreach (var parameter in Enum.GetValues(typeof(UnregisteredSchemaParameter))
                .Cast<UnregisteredSchemaParameter>())
            {
                switch (parameter)
                {
                    case UnregisteredSchemaParameter.SchemaReferences:
                        {
                            AppendParameter(parameters => parameters[(int)parameter] = new List<SchemaReference>());
                        }
                        break;
                    default:
                        {
                            AppendParameter(parameters =>
                            {
                                parameters[(int)parameter] = DefaultValueMappings[parameter].GetDefaultValue();
                            });
                        }
                        break;
                }
            }
        }

        public IUnregisteredSchemaBuilder FromConfiguration(string sectionKey)
        {
            if (!string.IsNullOrWhiteSpace(sectionKey))
            {
                var configurationSection = GetSection(sectionKey);

                foreach (var parameter in Enum.GetValues(typeof(UnregisteredSchemaParameter))
                    .Cast<UnregisteredSchemaParameter>())
                {
                    switch (parameter)
                    {
                        case UnregisteredSchemaParameter.SchemaReferences:
                            {
                                continue;
                            }
                        default:
                            {
                                AppendParameter(parameters =>
                                {
                                    var parameterType = DefaultValueMappings[parameter];
                                    var parameterKey = parameter.ToString();

                                    var parameterValue = configurationSection.GetValue(parameterType, parameterKey);

                                    parameters[(int)parameter] = parameterValue;
                                });
                            }
                            break;
                    }
                }
            }
            return this;
        }

        public IUnregisteredSchemaBuilder WithSchemaString(string schemaString)
        {
            AppendParameter(parameters => parameters[(int)UnregisteredSchemaParameter.SchemaString] = schemaString);
            return this;
        }

        public IUnregisteredSchemaBuilder WithSchemaReferences(List<SchemaReference> schemaReferences)
        {
            AppendParameter(parameters => parameters[(int)UnregisteredSchemaParameter.SchemaReferences] = schemaReferences ?? []);
            return this;
        }

        public IUnregisteredSchemaBuilder WithSchemaType(SchemaType schemaType)
        {
            AppendParameter(parameters => parameters[(int)UnregisteredSchemaParameter.SchemaType] = schemaType);
            return this;
        }

        public static Schema Build(IConfiguration configuration, Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema)
        {
            using var builder = new UnregisteredSchemaBuilder(configuration);

            configureUnregisteredSchema?.Invoke(builder);

            var unregisteredSchema = builder.Build();

            return unregisteredSchema;
        }
    }
}
