using System.Text.Json.Serialization;

namespace Chronicler.Internal
{
    internal class CK2PlayerCharacter
    {
        [JsonPropertyName("character_player_data")]
        public CK2PlayerCharacterData PlayerCharacterData { get; set; }
    }
}
