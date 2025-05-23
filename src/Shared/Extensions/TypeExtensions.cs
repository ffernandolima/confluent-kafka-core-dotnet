﻿using System;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Confluent.Kafka.Core.Internal
{
    internal static class TypeExtensions
    {
        private static readonly IDictionary<Type, string> BuiltInTypeNames =
            new Dictionary<Type, string>()
        {
            { typeof(void),    "void"    },
            { typeof(bool),    "bool"    },
            { typeof(byte),    "byte"    },
            { typeof(char),    "char"    },
            { typeof(decimal), "decimal" },
            { typeof(double),  "double"  },
            { typeof(float),   "float"   },
            { typeof(int),     "int"     },
            { typeof(long),    "long"    },
            { typeof(object),  "object"  },
            { typeof(sbyte),   "sbyte"   },
            { typeof(short),   "short"   },
            { typeof(string),  "string"  },
            { typeof(uint),    "uint"    },
            { typeof(ulong),   "ulong"   },
            { typeof(ushort),  "ushort"  }
        }
#if NET8_0_OR_GREATER
        .ToFrozenDictionary();
#else
        ;
#endif

        private static Dictionary<Type, object> DefaultValueTypes = [];

        public static string ExtractTypeName(this Type sourceType)
        {
            if (sourceType is null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (sourceType.IsGenericType)
            {
                var index = sourceType.Name.IndexOf('`');

                if (index > -1)
                {
                    var genericArguments = string.Join(", ", sourceType.GetGenericArguments().Select(ExtractTypeName));

                    var genericTypeName = $"{sourceType.Name.Remove(index)}<{genericArguments}>";

                    return genericTypeName;
                }
            }

            if (BuiltInTypeNames.TryGetValue(sourceType, out var builtInTypeName))
            {
                return builtInTypeName;
            }

            return sourceType.Name;
        }

        public static object GetDefaultValue(this Type sourceType)
        {
            if (sourceType is null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (!sourceType.IsValueType)
            {
                return null;
            }

            if (DefaultValueTypes.TryGetValue(sourceType, out var defaultValue))
            {
                return defaultValue;
            }

            defaultValue = Activator.CreateInstance(sourceType);

            Dictionary<Type, object> snapshot, newCache;

            do
            {
                snapshot = DefaultValueTypes;

                newCache = new Dictionary<Type, object>(DefaultValueTypes) { [sourceType] = defaultValue };

            } while (!ReferenceEquals(Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

            return defaultValue;
        }
    }
}
