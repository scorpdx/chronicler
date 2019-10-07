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

        public static async Task<ChronicleCollection> ParseJsonAsync(Stream jsonStream)
        {
            static async ValueTask<int> ReadPIDFromHeaderAsync(Stream stream)
            {
                var headerChunk = System.Buffers.ArrayPool<byte>.Shared.Rent(0x200);

                var bytesRead = await stream.ReadAsync(headerChunk, 0, headerChunk.Length);
                if (bytesRead < headerChunk.Length)
                    throw new InvalidOperationException("failed to read header");

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var noOptions = JsonSerializer.Deserialize<CK2txt>(headerChunk);
                sw.Stop();

                System.Buffers.ArrayPool<byte>.Shared.Return(headerChunk);

                return noOptions.Player.ID;
            }

            var xxx = await JsonSerializer.DeserializeAsync<CK2txt>(jsonStream, new JsonSerializerOptions
            {
                MaxDepth = 0
            });

            jsonStream.Seek(0, SeekOrigin.Begin);
            var yyy = await JsonSerializer.DeserializeAsync<CK2txt>(jsonStream, new JsonSerializerOptions
            {
                MaxDepth = 1
            });


            var ck2pcConverter = new CK2PlayerCharacterConverter();
            ck2pcConverter.PlayerID = await ReadPIDFromHeaderAsync(jsonStream);

            var options = new JsonSerializerOptions();
            options.Converters.Add(ck2pcConverter);

            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            var CK2txt = await JsonSerializer.DeserializeAsync<CK2txt>(jsonStream, options);
            sw2.Stop();

            throw new NotImplementedException();
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
