using System.Text.Json.Serialization;

namespace Chronicler.Internal
{
    internal class PlayerCharacterData
    {
        [JsonPropertyName("chronicle_collection")]
        public ChronicleCollection ChronicleCollection { get; set; }
    }
}
