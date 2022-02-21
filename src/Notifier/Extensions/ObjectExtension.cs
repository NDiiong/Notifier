using System;
using System.ComponentModel;

namespace Notifier.Extensions
{
    public static class ObjectExtension
    {
        public static T As<T>(this object @value)
        {
            return (T)As(@value, typeof(T));
        }

        /// <summary>
        /// Info: https://www.programmersought.com/article/31672222159/
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object As(this object @value, Type conversionType)
        {
            if (value == null)
                return default;

            if (@value is string _value)
            {
                if (conversionType == typeof(string))
                    return _value;

                if (conversionType == typeof(Guid))
                    return new Guid(Convert.ToString(_value));

                if (string.IsNullOrWhiteSpace(_value) && conversionType != typeof(string))
                    return default;

                if (conversionType == typeof(Version))
                    return new Version(_value);

                if (conversionType.IsEnum)
                {
                    if (Enum.IsDefined(conversionType, _value))
                        return Enum.Parse(conversionType, _value, true);

                    throw new InvalidOperationException("Invalid Cast Enum");
                }
            }
            else
            {
                if (conversionType == typeof(string))
                    return Convert.ToString(@value);
            }

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                conversionType = new NullableConverter(conversionType).UnderlyingType;

            if (conversionType.IsEnum && @value.GetType().IsValueType && !@value.GetType().IsEnum)
            {
                if (Enum.IsDefined(conversionType, @value))
                    return Enum.ToObject(conversionType, @value);

                throw new InvalidOperationException("Invalid Cast Enum");
            }

            if (conversionType is IConvertible || (conversionType.IsValueType && !conversionType.IsEnum))
                return Convert.ChangeType(@value, conversionType);

            return value;
        }
    }
}