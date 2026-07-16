using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public sealed class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "dd.MM.yyyy";

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                "Дата должна быть строковым значением.");
        }

        var dateString = reader.GetString();

        if (string.IsNullOrWhiteSpace(dateString))
        {
            throw new JsonException(
                "Дата не может быть пустой.");
        }

        if (!DateTime.TryParseExact(
                dateString,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            throw new JsonException(
                $"Дата должна иметь формат {DateFormat}.");
        }

        return date;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            value.ToString(
                DateFormat,
                CultureInfo.InvariantCulture));
    }
}