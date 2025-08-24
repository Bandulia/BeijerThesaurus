namespace Thesaurus.core.Entities
{
    // Represents a word entity in the thesaurus.
    // Each word has a unique identifier, a text value, and a collection of synonyms.
    public class Word
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public ICollection<WordSynonym> Synonyms { get; set; } = new List<WordSynonym>();
    }
}
