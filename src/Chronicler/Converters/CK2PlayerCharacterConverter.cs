using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler.Converters
{
    internal class CK2PlayerCharacterConverter : JsonConverter<CK2PlayerCharacter>
    {
        public int? PlayerID { get; set; }

        public override CK2PlayerCharacter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (PlayerID == null)
                throw new ArgumentNullException(nameof(PlayerID));

            var pid = PlayerID.ToString().AsSpan();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName when reader.ValueTextEquals(pid):
                        return JsonSerializer.Deserialize<CK2PlayerCharacter>(ref reader, options);
                    default:
                        reader.Skip();
                        break;
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, CK2PlayerCharacter value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}