using System.Text.Json.Serialization;

namespace Chronicler.Internal
{
    internal class CK2PlayerCharacterData
    {
        [JsonPropertyName("chronicle_collection")]
        public ChronicleCollection ChronicleCollection { get; set; }
    }
}
