using Chronicler.Internal;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler.Converters.Internal
{
    internal class CK2PlayerCharacterConverter : JsonConverter<CK2PlayerCharacter>
    {
        public int? PlayerID { get; set; }

        public override CK2PlayerCharacter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (PlayerID == null)
                throw new ArgumentNullException(nameof(PlayerID));

            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                case JsonTokenType.PropertyName:
                    if (!reader.Read())
                    {
                        throw new JsonException("expected token to parse");
                    }
                    break;
            }

            CK2PlayerCharacter result = null;
            var pid = PlayerID.ToString().AsSpan();
            var startingDepth = reader.CurrentDepth;
            do
            {
                if (!reader.Read())
                {
                    throw new JsonException("expected token to parse");
                }
                else if (result == null)
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.PropertyName when reader.ValueTextEquals(pid):
                            if (!reader.Read())
                            {
                                throw new JsonException("expected token to parse");
                            }
                            result = JsonSerializer.Deserialize<CK2PlayerCharacter>(ref reader); /* exclude options or we loop */
                            break;
                        default:
                            reader.TrySkip();
                            break;
                    }
                }
            }
            while (reader.CurrentDepth > startingDepth);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, CK2PlayerCharacter value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}