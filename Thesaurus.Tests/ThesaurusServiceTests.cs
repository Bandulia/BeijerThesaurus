using Thesaurus.core;
using Thesaurus.Core;

namespace Thesaurus.Tests
{
    public class ThesaurusServiceTests
    {
        private readonly IThesaurus _sut = new ThesaurusService();

        [Fact]
        public void AddWord_Builds_Bidirectional_Synonyms()
        {
            _sut.AddWord("happy", new[] { "joyful", "glad" });

            var happy = _sut.GetSynonyms("happy");
            Assert.Contains("joyful", happy);
            Assert.Contains("glad", happy);

            var joyful = _sut.GetSynonyms("joyful");
            Assert.Contains("happy", joyful);
            Assert.DoesNotContain("joyful", joyful);
        }

        [Fact]
        public void GetSynonyms_UnknownWord_Returns_Empty()
        {
            var s = _sut.GetSynonyms("nope");
            Assert.Empty(s);
        }

        [Fact]
        public void AddWord_Ignores_Duplicates_And_Whitespace()
        {
            _sut.AddWord("  Happy  ", new[] { "Joyful", "joyful", "  glad " });
            var s = _sut.GetSynonyms("happy");

            Assert.Equal(2, s.Count);
            Assert.Contains(s, x => string.Equals(x, "joyful", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(s, x => string.Equals(x, "glad", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void AddWord_Should_Not_Add_Self_As_Synonym()
        {
            _sut.AddWord("apple", new[] { "apple", "  apple  " });
            Assert.Empty(_sut.GetSynonyms("apple"));
        }

        [Fact]
        public void AddWord_Allows_Null_Synonyms_Safely()
        {
            _sut.AddWord("banana", null!);
            Assert.Empty(_sut.GetSynonyms("banana"));
        }

        [Fact]
        public void Adding_Same_Pair_Twice_Does_Not_Duplicate()
        {
            _sut.AddWord("fast", new[] { "quick" });
            _sut.AddWord("fast", new[] { "quick" });

            var fast = _sut.GetSynonyms("fast");
            Assert.Single(fast.Where(x => string.Equals(x, "quick", StringComparison.OrdinalIgnoreCase)));

            var quick = _sut.GetSynonyms("quick");
            Assert.Single(quick.Where(x => string.Equals(x, "fast", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void GetAllWords_Returns_Sorted_CaseInsensitive()
        {
            _sut.AddWord("Banana", new[] { "Plantain" });
            _sut.AddWord("apple", new[] { "pome" });
            _sut.AddWord("cherry", new[] { "fruit" });

            var words = _sut.GetAllWords().ToArray();
            var sorted = words.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
            Assert.True(words.SequenceEqual(sorted));
            Assert.Contains("apple", words);
            Assert.Contains("Banana", words);
            Assert.Contains("cherry", words);
        }

        [Fact]
        public void Returned_Collections_Are_Snapshots_Not_Live()
        {
            _sut.AddWord("sky", new[] { "azure" });
            var before = _sut.GetSynonyms("sky"); 

            _sut.AddWord("sky", new[] { "blue" });
            var after = _sut.GetSynonyms("sky");

            Assert.Contains("azure", before);
            Assert.DoesNotContain("blue", before);

            Assert.Contains("azure", after);
            Assert.Contains("blue", after);
        }

        [Theory]
        [InlineData("Happy", "happy", "JOYFUL")]
        [InlineData("HAPPY", "happy", "joyful")]
        [InlineData("happy", "HAPPY", "Joyful")]
        public void Case_Insensitive_Across_All_Operations(string inputWord, string queryWord, string synonymVariant)
        {
            _sut.AddWord(inputWord, new[] { synonymVariant });
            var s = _sut.GetSynonyms(queryWord);

            Assert.Contains(s, x => string.Equals(x, "joyful", StringComparison.OrdinalIgnoreCase));
        }
    }
}
