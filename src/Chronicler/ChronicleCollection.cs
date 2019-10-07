using Chronicler.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            //var chronicleElements = chronicleCollection
            //    .EnumerateObject()
            //    .Where(prop => prop.NameEquals("chronicle"))
            //    .Select(prop => prop.Value);

            //var chronicles = chronicleElements
            //    .Select(chronicle => new
            //    {
            //        chronicle,
            //        chapters = chronicle.EnumerateObject()
            //            .Where(prop => prop.NameEquals("chronicle_chapter"))
            //            .Select(prop => prop.Value).Select(chapter => new
            //            {
            //                entries = chapter.EnumerateObject()
            //                    .Where(prop => prop.NameEquals("chronicle_entry"))
            //                    .Select(prop => prop.Value)
            //                    .Select(entry => new
            //                    {
            //                        entry,
            //                        text = entry.GetProperty("text").GetString(),
            //                        //pi
            //                    }),
            //                year = chapter.GetProperty("year").GetInt32()
            //            })
            //    });

            return new JsonElementSerializer(chronicleCollection).ToObject<ChronicleCollection>();
        }

        public static async Task<ChronicleCollection> ParseJsonAsync(Stream jsonStream)
        {
            CK2txt ck;
            ck = await JsonSerializer.DeserializeAsync<CK2txt>(jsonStream);
            jsonStream.Seek(0, SeekOrigin.Begin);

            var ck2pcConverter = new CK2PlayerCharacterConverter();
            ck2pcConverter.PlayerID = ck.Player.ID;

            var options = new JsonSerializerOptions();
            options.Converters.Add(ck2pcConverter);

            ck = await JsonSerializer.DeserializeAsync<CK2txt>(jsonStream, options);
            return ck.PlayerCharacter.PlayerCharacterData.ChronicleCollection;
        }
    }

    internal class CK2txt
    {
        [JsonPropertyName("player")]
        public CK2Player Player { get; set; }

        [JsonPropertyName("character")]
        public CK2PlayerCharacter PlayerCharacter { get; set; }
    }

    internal class CK2Player
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
    }

    internal class CK2PlayerCharacter
    {
        [JsonPropertyName("character_player_data")]
        public PlayerCharacterData PlayerCharacterData { get; set; }
    }

    internal class PlayerCharacterData
    {
        [JsonPropertyName("chronicle_collection")]
        public ChronicleCollection ChronicleCollection { get; set; }
    }
}
