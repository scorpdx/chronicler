using Chronicler.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler
{
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

            return new JsonElementSerializer(chronicleCollection).ToObject<ChronicleCollection>();
        }
    }
}
