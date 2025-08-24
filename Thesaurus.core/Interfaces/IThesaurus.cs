namespace Thesaurus.core.Interfaces
{
    public interface IThesaurus
    {
        // Adds a word along with its synonyms to the thesaurus.
        void AddWord(string word, IEnumerable<string> synonyms);

        // Retrieves a read-only collection of synonyms for the specified word.
        IReadOnlyCollection<string> GetSynonyms(string word);

        // Retrieves a read-only collection of all words in the thesaurus.
        IReadOnlyCollection<string> GetAllWords();
    }
}
