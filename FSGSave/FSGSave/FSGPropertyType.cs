using System;
using System.Globalization;

namespace FSGSave
{
    public enum FSGPropertyType : int
    {
        Bool = 0x01,
        Int = 0x02,
        Uint = 0x03,
        Uint64 = 0x05,
        Float = 0x06,
        Collection = 0x0C,
    }

    public static class FSGPropertyTypeExtensions
    {
        public static string ToString(this FSGPropertyType type, object value)
        {
            return type.ToString(value, CultureInfo.InvariantCulture);
        }

        public static string ToString(this FSGPropertyType type, object value, IFormatProvider provider)
        {
            switch (type)
            {
                case FSGPropertyType.Bool:
                    return ((bool)value).ToString(provider);
                case FSGPropertyType.Int:
                    return ((int)value).ToString(provider);
                case FSGPropertyType.Uint:
                    return ((uint)value).ToString(provider);
                case FSGPropertyType.Uint64:
                    return ((ulong)value).ToString(provider);
                case FSGPropertyType.Float:
                    return ((float)value).ToString(provider);
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool TryParse(this FSGPropertyType type, string stringValue, out object value)
        {
            switch (type)
            {
                case FSGPropertyType.Int:
                case FSGPropertyType.Uint:
                case FSGPropertyType.Uint64:
                    return type.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                case FSGPropertyType.Float:
                    return type.TryParse(stringValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);
                default:
                    return type.TryParse(stringValue, NumberStyles.None, CultureInfo.InvariantCulture, out value);

            }
        }

        public static bool TryParse(this FSGPropertyType type, string stringValue,
            NumberStyles style, IFormatProvider provider, out object value)
        {
            bool parseResult;

            switch (type)
            {
                case FSGPropertyType.Bool:
                    parseResult = bool.TryParse(stringValue, out var boolValue);
                    value = boolValue;
                    return parseResult;
                case FSGPropertyType.Int:
                    parseResult = int.TryParse(stringValue, style, provider, out var intValue);
                    value = intValue;
                    return parseResult;
                case FSGPropertyType.Uint:
                    parseResult = uint.TryParse(stringValue, style, provider, out var uintValue);
                    value = uintValue;
                    return parseResult;
                case FSGPropertyType.Uint64:
                    parseResult = ulong.TryParse(stringValue, style, provider, out var uint64Value);
                    value = uint64Value;
                    return parseResult;
                case FSGPropertyType.Float:
                    parseResult = float.TryParse(stringValue, style, provider, out var floatValue);
                    value = floatValue;
                    return parseResult;
                default:
                    throw new NotImplementedException();
            }
        }

        public static object GetDefaultValue(this FSGPropertyType type)
        {
            switch (type)
            {
                case FSGPropertyType.Bool:
                    return default(bool);
                case FSGPropertyType.Int:
                    return default(int);
                case FSGPropertyType.Uint:
                    return default(uint);
                case FSGPropertyType.Uint64:
                    return default(ulong);
                case FSGPropertyType.Float:
                    return default(float);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
