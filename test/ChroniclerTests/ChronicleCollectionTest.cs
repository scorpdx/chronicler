using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace ChroniclerTests
{
    public class ChronicleCollectionTest
    {
        private static string CK2JsonPath => Path.Combine("resources", "Ironman_West_Francia_HaR.json");

        private static readonly JsonDocument CK2Json = JsonDocument.Parse(File.ReadAllText(CK2JsonPath));

        [Fact]
        public void TestParseJson()
        {
            var chronicleCollection = Chronicler.ChronicleCollection.ParseJson(CK2Json);
            Assert.False(string.IsNullOrEmpty(chronicleCollection.Chronicles[0].Chapters[0].Entries[0].Text));

            Assert.Collection(chronicleCollection.Chronicles[0].Chapters,
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries));
        }
    }
}
