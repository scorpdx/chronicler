using Chronicler.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Chronicler
{
    [JsonConverter(typeof(ChronicleConverter))]
    public class Chronicle
    {
        [JsonPropertyName("chronicle_chapter")]
        public List<ChronicleChapter> Chapters { get; set; }

        public int Character { get; set; }
    }
}
