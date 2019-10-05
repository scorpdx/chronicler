using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler.Converters
{
    public class ChronicleConverter : JsonConverter<Chronicle>
    {
        public override Chronicle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var chapters = new List<ChronicleChapter>();
            int? character = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName when reader.ValueTextEquals("chronicle_chapter"):
                        var chapter = JsonSerializer.Deserialize<ChronicleChapter>(ref reader, options);
                        chapters.Add(chapter);
                        break;
                    case JsonTokenType.PropertyName when reader.ValueTextEquals("character"):
                        reader.Read();
                        character = reader.GetInt32();
                        break;
                }
            }

            if (!character.HasValue)
                throw new JsonException($"required property `{nameof(character)}` was not found");

            return new Chronicle
            {
                Chapters = chapters,
                Character = character.Value
            };
        }

        public override void Write(Utf8JsonWriter writer, Chronicle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
