using Microsoft.EntityFrameworkCore;
using Thesaurus.core.Entities;
using Thesaurus.Core.Data;
using Thesaurus.core.Interfaces;

namespace Thesaurus.core.Services;

public class ThesaurusServiceEf : IThesaurus
{
    private readonly ThesaurusDbContext _db;

    public ThesaurusServiceEf(ThesaurusDbContext db)
    {
        _db = db;
    }

    public void AddWord(string word, IEnumerable<string> synonyms)
    {
        // Check if the word already exists in the database  
        var w = _db.Words.SingleOrDefault(x => x.Text == word);
        if (w == null)
        {
            // If the word does not exist, create and add it to the database  
            w = new Word { Text = word };
            _db.Words.Add(w);
        }

        foreach (var syn in synonyms)
        {
            // Check if the synonym already exists in the database, if not, create it  
            var s = _db.Words.SingleOrDefault(x => x.Text == syn) ?? new Word { Text = syn };
            if (s.Id == 0) _db.Words.Add(s);

            // Ensure the synonym relationship does not already exist before adding  
            if (!_db.WordSynonyms.Any(ws => ws.WordId == w.Id && ws.SynonymId == s.Id))
            {
                // Add the synonym relationship in both directions  
                _db.WordSynonyms.Add(new WordSynonym { Word = w, Synonym = s });
                _db.WordSynonyms.Add(new WordSynonym { Word = s, Synonym = w });
            }
        }

        _db.SaveChanges();
    }

    public IReadOnlyCollection<string> GetSynonyms(string word)
    {
        // Retrieve the word along with its synonyms from the database  
        var w = _db.Words
            .Include(x => x.Synonyms)
            .ThenInclude(ws => ws.Synonym)
            .SingleOrDefault(x => x.Text == word);

        // Return the list of synonyms or an empty array if the word is not found  
        return w?.Synonyms.Select(ws => ws.Synonym!.Text).ToArray()
               ?? Array.Empty<string>();
    }

    public IReadOnlyCollection<string> GetAllWords()
    {
        // Retrieve all words from the database, sorted alphabetically  
        return _db.Words.Select(x => x.Text).OrderBy(x => x).ToArray();
    }
}
