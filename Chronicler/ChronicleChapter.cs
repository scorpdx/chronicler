using Chronicler.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Chronicler
{
    [JsonConverter(typeof(ChronicleChapterConverter))]
    public class ChronicleChapter
    {
        [JsonPropertyName("chronicle_entry")]
        public List<ChronicleEntry> Entries { get; set; }

        public int Year { get; set; }
    }
}
