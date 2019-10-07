using Chronicler;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ChroniclerTests
{
    public class ChronicleCollectionTest
    {
        private static string CK2JsonPath => Path.Combine("resources", "Ironman_West_Francia_HaR.json");

        private static Stream GetCK2JsonStream() => File.OpenRead(CK2JsonPath);
        private static readonly JsonDocument CK2Json = JsonDocument.Parse(File.ReadAllText(CK2JsonPath));

        [Fact]
        public void TestParseJson()
        {
            var chronicleCollection = Chronicler.ChronicleCollection.ParseJson(CK2Json);
            VerifyJson(chronicleCollection);
        }

        [Fact]
        public async Task TestParseJsonAsync()
        {
            using var stream = GetCK2JsonStream();
            var chronicleCollection = await Chronicler.ChronicleCollection.ParseJsonAsync(stream);
            VerifyJson(chronicleCollection);
        }

        private void VerifyJson(ChronicleCollection chronicleCollection)
        {
            Assert.NotNull(chronicleCollection);
            Assert.NotEmpty(chronicleCollection.Chronicles);
            Assert.Collection(chronicleCollection.Chronicles[0].Chapters,
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries),
                chap => Assert.NotEmpty(chap.Entries));
            Assert.False(string.IsNullOrEmpty(chronicleCollection.Chronicles[0].Chapters[0].Entries[0].Text));
        }
    }
}
