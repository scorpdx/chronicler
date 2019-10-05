using System;
using System.IO;
using System.Text.Json;
using Xunit;

namespace ChroniclerTests
{
    public class ChronicleCollectionTest
    {
        private static JsonDocument GetCK2Json() => JsonDocument.Parse(File.ReadAllText(@"C:\Users\scorp\Desktop\ck2kok.json"));

        [Fact]
        public void TestParseJson()
        {
            var chronicleCollection = Chronicler.ChronicleCollection.ParseJson(GetCK2Json());
            Assert.False(string.IsNullOrEmpty(chronicleCollection.Chronicles[0].Chapters[0].Entries[0].Text));
        }
    }
}
