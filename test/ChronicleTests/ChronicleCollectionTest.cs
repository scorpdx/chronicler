using System;
using System.IO;
using System.Text.Json;
using Xunit;

namespace ChronicleTests
{
    public class ChronicleCollectionTest
    {
        private static JsonDocument GetCK2Json() => JsonDocument.Parse(File.ReadAllText(@"C:\Users\scorp\Desktop\ck2kok.json"));

        [Fact]
        public void TestParse()
        {
            var chronicle = Chronicler.ChronicleCollection.ParseJson(GetCK2Json());
            Assert.NotNull(chronicle);
        }
    }
}
