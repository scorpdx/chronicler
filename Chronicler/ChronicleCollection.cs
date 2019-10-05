using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler
{
    internal class ChronicleCollectionConverter : JsonConverter<ChronicleCollection>
    {
        //JsonConverter<T> GetConverter<T>() => options.Converters
        //    .Where(converter => converter.CanConvert(typeof(T)))
        //    .OfType<JsonConverter<T>>()
        //    .First();

        public override ChronicleCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var chronicles = new List<Chronicle>();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName when reader.ValueTextEquals("chronicle"):
                        var chronicle = JsonSerializer.Deserialize<Chronicle>(ref reader, options);
                        chronicles.Add(chronicle);
                        break;
                }
            }

            return new ChronicleCollection
            {
                Chronicles = chronicles
            };
        }

        public override void Write(Utf8JsonWriter writer, ChronicleCollection value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChronicleConverter : JsonConverter<Chronicle>
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

    public class ChronicleEntry
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("portrait")]
        public int Portrait { get; set; }

        [JsonPropertyName("portrait_culture")]
        public string PortraitCulture { get; set; }

        [JsonPropertyName("portrait_title_tier")]
        public int PortraitTitleTier { get; set; }

        [JsonPropertyName("portrait_government")]
        public string PortraitGovernment { get; set; }
    }

    [JsonConverter(typeof(ChronicleChapterConverter))]
    public class ChronicleChapter
    {
        [JsonPropertyName("chronicle_entry")]
        public List<ChronicleEntry> Entries { get; set; }

        public int Year { get; set; }
    }

    [JsonConverter(typeof(ChronicleConverter))]
    public class Chronicle
    {
        [JsonPropertyName("chronicle_chapter")]
        public List<ChronicleChapter> Chapters { get; set; }

        public int Character { get; set; }
    }

    [JsonConverter(typeof(ChronicleCollectionConverter))]
    public class ChronicleCollection
    {
        [JsonPropertyName("chronicle")]
        public List<Chronicle> Chronicles { get; set; }

        public static ChronicleCollection ParseJson(JsonDocument ck2json)
        {
            var playerId = ck2json.RootElement
                .GetProperty("player")
                .GetProperty("id")
                .GetRawText();

            var playerCharacterElement = ck2json.RootElement
                .GetProperty("character")
                .GetProperty(playerId);

            var chronicleCollection = playerCharacterElement
                .GetProperty("character_player_data")
                .GetProperty("chronicle_collection");

            var chronicleElements = chronicleCollection
                .EnumerateObject()
                .Where(prop => prop.NameEquals("chronicle"))
                .Select(prop => prop.Value);

            var chronicles = chronicleElements
                .Select(chronicle => new
                {
                    chronicle,
                    chapters = chronicle.EnumerateObject()
                        .Where(prop => prop.NameEquals("chronicle_chapter"))
                        .Select(prop => prop.Value).Select(chapter => new
                        {
                            entries = chapter.EnumerateObject()
                                .Where(prop => prop.NameEquals("chronicle_entry"))
                                .Select(prop => prop.Value)
                                .Select(entry => new
                                {
                                    entry,
                                    text = entry.GetProperty("text").GetString(),
                                    //pi
                                }),
                            year = chapter.GetProperty("year").GetInt32()
                        })
                });

            return JsonSerializer.Deserialize<ChronicleCollection>(chronicleCollection.ToString());
        }
    }
}
