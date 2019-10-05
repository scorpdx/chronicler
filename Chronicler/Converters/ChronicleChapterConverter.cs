using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler.Converters
{
    internal class ChronicleChapterConverter : JsonConverter<ChronicleChapter>
    {
        public override ChronicleChapter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var entries = new List<ChronicleEntry>();
            int? year = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName when reader.ValueTextEquals("chronicle_entry"):
                        var entry = JsonSerializer.Deserialize<ChronicleEntry>(ref reader, options);
                        entries.Add(entry);
                        break;
                    case JsonTokenType.PropertyName when reader.ValueTextEquals("year"):
                        reader.Read();
                        year = reader.GetInt32();
                        break;
                }
            }

            if (!year.HasValue)
                throw new JsonException($"required property `{nameof(year)}` was not found");

            return new ChronicleChapter
            {
                Entries = entries,
                Year = year.Value
            };
        }

        public override void Write(Utf8JsonWriter writer, ChronicleChapter value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
