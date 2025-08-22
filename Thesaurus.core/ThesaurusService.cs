namespace Thesaurus.Core
{
    public class ThesaurusService : IThesaurus
    {
 
        private Dictionary<string, HashSet<string>> _thesaurusDictionary = new(StringComparer.OrdinalIgnoreCase);
        private SortedSet<string> _allWords = new(StringComparer.OrdinalIgnoreCase);

        // Adding a word and its synonyms if any
        public void AddWord(string word, IEnumerable<string> synonyms)
        {
            if (string.IsNullOrWhiteSpace(word)) return;

            var w = word.Trim();

            // Making sure the corresponding value for the key becomes a HashSet not the default value
            if (!_thesaurusDictionary.TryGetValue(w, out var wSet))
            {
                wSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _thesaurusDictionary[w] = wSet;
                _allWords.Add(w);
            }

            // Adding the synonyms one by one, handling null and empty indexes 
            foreach (var raw in synonyms ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var syn = raw.Trim();
                if (syn.Equals(w, StringComparison.OrdinalIgnoreCase)) continue;

                wSet.Add(syn);

                if (!_thesaurusDictionary.TryGetValue(syn, out var synSet))
                {
                    synSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _thesaurusDictionary[syn] = synSet;
                    _allWords.Add(syn);
                }

                synSet.Add(w);
            }
        }

        // Returns the synonyms of the word in order
        public IReadOnlyCollection<string> GetSynonyms(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return Array.Empty<string>();

            var w = word.Trim();

            if (_thesaurusDictionary.TryGetValue(w, out var set))
            {
                return set.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
            }

            return Array.Empty<string>();
        }

        // Return all of the words in the dictionary in order 
        public IReadOnlyCollection<string> GetAllWords()
        {
            return _allWords.ToArray();
        }
    }
}
