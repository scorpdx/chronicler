using System.Text.Json.Serialization;

namespace Chronicler.Internal
{
    internal class CK2Player
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
    }
}
