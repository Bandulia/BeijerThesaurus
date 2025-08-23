namespace Thesaurus.core.Interfaces
{
    public interface IThesaurus
    {
        void AddWord(string word, IEnumerable<string> synonyms);

        IReadOnlyCollection<string> GetSynonyms(string word);

        IReadOnlyCollection<string> GetAllWords();
    }
}
