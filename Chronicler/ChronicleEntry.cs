using System.Text.Json.Serialization;

namespace Chronicler
{
    public class ChronicleEntry
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("portrait")]
        public int Portrait { get; set; }

        [JsonPropertyName("portrait_culture")]
        public string PortraitCulture { get; set; }

        [JsonPropertyName("portrait_title_tier")]
        public int PortraitTitleTier { get; set; }

        [JsonPropertyName("portrait_government")]
        public string PortraitGovernment { get; set; }
    }
}
