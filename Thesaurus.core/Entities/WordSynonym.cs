namespace Thesaurus.core.Entities
{
    // Represents a many-to-many relationship between words and their synonyms.
    // Each instance links a word to one of its synonyms.
    public class WordSynonym
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public int SynonymId { get; set; }

        public Word? Word { get; set; }
        public Word? Synonym { get; set; }
    }
}
