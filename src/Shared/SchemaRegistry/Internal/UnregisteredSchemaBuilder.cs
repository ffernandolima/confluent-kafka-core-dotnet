﻿using Confluent.Kafka.Core.Internal;
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
            EnumerateParameters(parameter =>
            {
                if (parameter == UnregisteredSchemaParameter.SchemaReferences)
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

        public IUnregisteredSchemaBuilder FromConfiguration(string sectionKey)
        {
            if (!string.IsNullOrWhiteSpace(sectionKey))
            {
                var section = GetSection(sectionKey);

                EnumerateParameters(parameter => parameter != UnregisteredSchemaParameter.SchemaReferences, parameter =>
                {
                    AppendParameter(parameters =>
                    {
                        parameters[(int)parameter] = section.GetValue(DefaultValueMappings[parameter], parameter.ToString());
                    });
                });
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

        private void EnumerateParameters(Action<UnregisteredSchemaParameter> action) => EnumerateParameters(condition: null, action);

        private void EnumerateParameters(Func<UnregisteredSchemaParameter, bool> condition, Action<UnregisteredSchemaParameter> action)
        {
            foreach (var parameter in Enum.GetValues(typeof(UnregisteredSchemaParameter))
                .Cast<UnregisteredSchemaParameter>())
            {
                if (condition is null || condition.Invoke(parameter))
                {
                    action?.Invoke(parameter);
                }
            }
        }
    }
}
