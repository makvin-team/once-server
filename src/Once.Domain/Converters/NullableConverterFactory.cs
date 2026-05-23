using System.Text.Json;
using System.Text.Json.Serialization;

namespace Once.Domain.Converters;

public class EmptyStringAsNullConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // If the JSON token is a string...
        if (reader.TokenType == JsonTokenType.String)
        {
            // ...and it's an empty string, return the default value (which is null).
            if (reader.GetString() == string.Empty)
            {
                return default;
            }
        }

        // Use the default converter for any other value.
        // The serializer will handle the conversion from the current token.
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Use the default converter for serialization.
        JsonSerializer.Serialize(writer, value, options);
    }
}