using Chronicler.Converters;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chronicler
{
    [JsonConverter(typeof(ChronicleCollectionConverter))]
    public class ChronicleCollection
    {
        [JsonPropertyName("chronicle")]
        public List<Chronicle> Chronicles { get; set; }

        public static ChronicleCollection Parse(JsonDocument jsonDoc)
        {
            var playerId = jsonDoc.RootElement
                .GetProperty("player")
                .GetProperty("id")
                .GetRawText();

            var playerCharacterElement = jsonDoc.RootElement
                .GetProperty("character")
                .GetProperty(playerId);

            var chronicleCollection = playerCharacterElement
                .GetProperty("character_player_data")
                .GetProperty("chronicle_collection");

            return new JsonElementSerializer(chronicleCollection).ToObject<ChronicleCollection>();
        }

        public static ChronicleCollection Parse(Stream jsonStream)
        {
            using var reader = new StreamReader(jsonStream);
            return Parse(reader.ReadToEnd());
        }

        public static ChronicleCollection Parse(string jsonText)
            => JsonSerializer.Deserialize<ChronicleCollection>(jsonText);

        public static ValueTask<ChronicleCollection> ParseAsync(Stream jsonStream)
            => JsonSerializer.DeserializeAsync<ChronicleCollection>(jsonStream);
    }
}
