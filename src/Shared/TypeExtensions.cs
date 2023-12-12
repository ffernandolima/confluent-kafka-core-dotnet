using System;
using System.Collections.Generic;
using System.Linq;

namespace Confluent.Kafka.Core.Internal
{
    internal static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> BuiltInTypeNames = new()
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
        };

        public static string ExtractTypeName(this Type sourceType)
        {
            if (sourceType is null)
            {
                throw new ArgumentNullException(nameof(sourceType), $"{nameof(sourceType)} cannot be null.");
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
    }
}
