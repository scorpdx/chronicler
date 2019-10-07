using Chronicler.Converters;
using Chronicler.Converters.Internal;
using Chronicler.Internal;
using System;
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

            return new JsonElementSerializer(chronicleCollection).ToObject<ChronicleCollection>();
        }

        public static async Task<ChronicleCollection> ParseJsonAsync(Stream jsonStream)
        {
            static int ReadPIDFromHeader(ReadOnlySpan<byte> headerChunk)
            {
                var reader = new Utf8JsonReader(headerChunk);

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.PropertyName when reader.ValueTextEquals("id"):
                            if (reader.Read())
                            {
                                return reader.GetInt32();
                            }
                            goto fail;
                    }
                }

            fail:
                throw new JsonException("couldn't find player ID");
            }

            int pid;
            {
                var headerChunk = System.Buffers.ArrayPool<byte>.Shared.Rent(0x200);

                var bytesRead = await jsonStream.ReadAsync(headerChunk, 0, headerChunk.Length);
                if (bytesRead < headerChunk.Length)
                    throw new InvalidOperationException("failed to read header");

                pid = ReadPIDFromHeader(headerChunk);

                System.Buffers.ArrayPool<byte>.Shared.Return(headerChunk);
            }
            jsonStream.Seek(0, SeekOrigin.Begin);

            var ck2pcConverter = new CK2PlayerCharacterConverter
            {
                PlayerID = pid
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(ck2pcConverter);

            var ck = await JsonSerializer.DeserializeAsync<CK2txt>(jsonStream, options);
            return ck.PlayerCharacter.PlayerCharacterData.ChronicleCollection;
        }
    }
}
