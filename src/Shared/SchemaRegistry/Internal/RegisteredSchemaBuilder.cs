﻿using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using System;
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

        private static readonly Dictionary<RegisteredSchemaParameter, Type> DefaultValueMappings = new()
        {
            { RegisteredSchemaParameter.Subject,          typeof(string)                },
            { RegisteredSchemaParameter.Version,          typeof(int)                   },
            { RegisteredSchemaParameter.Id,               typeof(int)                   },
            { RegisteredSchemaParameter.SchemaString,     typeof(string)                },
            { RegisteredSchemaParameter.SchemaType,       typeof(SchemaType)            },
            { RegisteredSchemaParameter.SchemaReferences, typeof(List<SchemaReference>) }
        };

        public RegisteredSchemaBuilder(IConfiguration configuration = null)
           : base(seedSubject: null, configuration)
        {
            foreach (var parameter in Enum.GetValues(typeof(RegisteredSchemaParameter))
                .Cast<RegisteredSchemaParameter>())
            {
                switch (parameter)
                {
                    case RegisteredSchemaParameter.SchemaReferences:
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

        public IRegisteredSchemaBuilder FromConfiguration(string sectionKey)
        {
            if (!string.IsNullOrWhiteSpace(sectionKey))
            {
                var configurationSection = GetSection(sectionKey);

                foreach (var parameter in Enum.GetValues(typeof(RegisteredSchemaParameter))
                    .Cast<RegisteredSchemaParameter>())
                {
                    switch (parameter)
                    {
                        case RegisteredSchemaParameter.SchemaReferences:
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
    }
}
