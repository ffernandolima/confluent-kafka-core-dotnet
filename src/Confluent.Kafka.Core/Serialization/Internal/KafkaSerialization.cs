using System;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Serialization.Internal
{
    internal static class KafkaSerialization
    {
        public static readonly ISerializer<Ignore> Ignore = new IgnoreSerializer();

        public static readonly IDictionary<Type, object> DefaultDeserializers =
            new Dictionary<Type, object>()
        {
            { typeof(Ignore), Deserializers.Ignore    },
            { typeof(Null),   Deserializers.Null      },
            { typeof(int),    Deserializers.Int32     },
            { typeof(long),   Deserializers.Int64     },
            { typeof(string), Deserializers.Utf8      },
            { typeof(float),  Deserializers.Single    },
            { typeof(double), Deserializers.Double    },
            { typeof(byte[]), Deserializers.ByteArray }
        }
#if NET8_0_OR_GREATER
        .ToFrozenDictionary();
#else
        ;
#endif

        public static readonly IDictionary<Type, object> DefaultSerializers =
            new Dictionary<Type, object>()
        {
            { typeof(Ignore), Ignore                },
            { typeof(Null),   Serializers.Null      },
            { typeof(int),    Serializers.Int32     },
            { typeof(long),   Serializers.Int64     },
            { typeof(string), Serializers.Utf8      },
            { typeof(float),  Serializers.Single    },
            { typeof(double), Serializers.Double    },
            { typeof(byte[]), Serializers.ByteArray }
        }
#if NET8_0_OR_GREATER
        .ToFrozenDictionary();
#else
        ;
#endif
        private sealed class IgnoreSerializer : ISerializer<Ignore>
        {
            public byte[] Serialize(Ignore data, SerializationContext context) => null;
        }

        public static object TryGetDeserializer(Type type)
        {
            if (DefaultDeserializers.TryGetValue(type, out object deserializer))
            {
                return deserializer;
            }

            return null;
        }

        public static IDeserializer<T> TryGetDeserializer<T>()
        {
            if (TryGetDeserializer(typeof(T)) is IDeserializer<T> deserializer)
            {
                return deserializer;
            }

            return null;
        }

        public static object TryGetSerializer(Type type)
        {
            if (DefaultSerializers.TryGetValue(type, out object serializer))
            {
                return serializer;
            }

            return null;
        }

        public static ISerializer<T> TryGetSerializer<T>()
        {
            if (TryGetSerializer(typeof(T)) is ISerializer<T> serializer)
            {
                return serializer;
            }

            return null;
        }
    }
}
