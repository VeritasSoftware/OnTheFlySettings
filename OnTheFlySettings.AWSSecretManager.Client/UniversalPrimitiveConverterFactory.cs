using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnTheFlySettings.AWSSecretManager.Client
{
    public class UniversalPrimitiveConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            Type t = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
            return t.IsPrimitive || t == typeof(decimal) || t == typeof(string);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            bool isNullable = Nullable.GetUnderlyingType(typeToConvert) != null;
            Type underlyingType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

            if (underlyingType == typeof(string))
            {
                return isNullable
                    ? new UniversalStringNullableConverter()
                    : new UniversalStringNonNullableConverter();
            }
            else
            {
                Type converterType = isNullable
                    ? typeof(UniversalPrimitiveNullableConverter<>).MakeGenericType(underlyingType)
                    : typeof(UniversalPrimitiveNonNullableConverter<>).MakeGenericType(underlyingType);

                return (JsonConverter)Activator.CreateInstance(converterType)!;
            }
        }

        // Nullable value type converter
        private class UniversalPrimitiveNullableConverter<T> : JsonConverter<T?> where T : struct, IConvertible
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                object? value = ReadValue(ref reader, typeof(T));
                return (T?)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                    WriteValue(writer, value.Value);
                else
                    writer.WriteNullValue();
            }
        }

        // Non-nullable value type converter
        private class UniversalPrimitiveNonNullableConverter<T> : JsonConverter<T> where T : struct, IConvertible
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                object? value = ReadValue(ref reader, typeof(T));
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                WriteValue(writer, value);
            }
        }

        // Nullable string converter
        private class UniversalStringNullableConverter : JsonConverter<string?>
        {
            public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                return reader.GetString();
            }

            public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
            {
                if (value != null)
                    writer.WriteStringValue(value);
                else
                    writer.WriteNullValue();
            }
        }

        // Non-nullable string converter
        private class UniversalStringNonNullableConverter : JsonConverter<string>
        {
            public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetString() ?? string.Empty;
            }

            public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        // Shared read logic for value types
        private static object? ReadValue(ref Utf8JsonReader reader, Type targetType)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    if (targetType == typeof(bool))
                        return reader.GetDouble() != 0;
                    return reader.GetDouble();

                case JsonTokenType.String:
                    string? str = reader.GetString();
                    if (string.IsNullOrWhiteSpace(str))
                        return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

                    if (targetType == typeof(bool))
                    {
                        if (bool.TryParse(str, out bool b)) return b;
                        if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
                            return num != 0;
                    }

                    if (targetType == typeof(char) && str.Length == 1)
                        return str[0];

                    if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedNum))
                        return parsedNum;

                    throw new JsonException($"Invalid string for {targetType.Name}: '{str}'");

                case JsonTokenType.True:
                    if (targetType == typeof(bool)) return true;
                    return 1;

                case JsonTokenType.False:
                    if (targetType == typeof(bool)) return false;
                    return 0;

                case JsonTokenType.StartArray:
                case JsonTokenType.StartObject:
                    reader.Skip();
                    return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

                default:
                    throw new JsonException($"Unsupported token {reader.TokenType} for {targetType.Name}.");
            }
        }

        // Shared write logic for value types
        private static void WriteValue<T>(Utf8JsonWriter writer, T value) where T : IConvertible
        {
            switch (value)
            {
                case bool b:
                    writer.WriteBooleanValue(b);
                    break;
                case char c:
                    writer.WriteStringValue(c.ToString());
                    break;
                default:
                    writer.WriteNumberValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    break;
            }
        }
    }
}
