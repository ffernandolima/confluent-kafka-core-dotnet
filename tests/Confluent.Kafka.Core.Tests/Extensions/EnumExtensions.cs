using System;
using System.ComponentModel;

namespace Confluent.Kafka.Core.Tests.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            var type = typeof(TEnum);

            var field = type.GetField(value.ToString());

            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute is null ? value.ToString() : attribute.Description;
        }
    }
}