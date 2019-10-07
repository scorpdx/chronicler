using System.Text.Json.Serialization;

namespace Chronicler.Internal
{
    internal class CK2txt
    {
        [JsonPropertyName("player")]
        public CK2Player Player { get; set; }

        [JsonPropertyName("character")]
        public CK2PlayerCharacter PlayerCharacter { get; set; }
    }
}
