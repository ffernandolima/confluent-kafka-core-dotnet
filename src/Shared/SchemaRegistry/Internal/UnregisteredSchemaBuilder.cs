using Confluent.Kafka.Core.Internal;
using Confluent.SchemaRegistry;
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

        public UnregisteredSchemaBuilder(RegisteredSchema seedSubject = null)
           : base(seedSubject)
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

        public IUnregisteredSchemaBuilder WithSchemaString(string schemaString)
        {
            AppendParameter(parameters => parameters[(int)UnregisteredSchemaParameter.SchemaString] = schemaString);
            return this;
        }

        public IUnregisteredSchemaBuilder WithSchemaReferences(List<SchemaReference> schemaReferences)
        {
            AppendParameter(parameters =>
            {
                parameters[(int)UnregisteredSchemaParameter.SchemaReferences] = schemaReferences ?? new List<SchemaReference>();
            });
            return this;
        }

        public IUnregisteredSchemaBuilder WithSchemaType(SchemaType schemaType)
        {
            AppendParameter(parameters => parameters[(int)UnregisteredSchemaParameter.SchemaType] = schemaType);
            return this;
        }

        public static Schema Build(Action<IUnregisteredSchemaBuilder> configureUnregisteredSchema)
        {
            using var builder = new UnregisteredSchemaBuilder();

            configureUnregisteredSchema?.Invoke(builder);

            var unregisteredSchema = builder.Build();

            return unregisteredSchema;
        }
    }
}
