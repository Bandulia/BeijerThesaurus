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
        var w = _db.Words.SingleOrDefault(x => x.Text == word);
        if (w == null)
        {
            w = new Word { Text = word };
            _db.Words.Add(w);
        }

        foreach (var syn in synonyms)
        {
            var s = _db.Words.SingleOrDefault(x => x.Text == syn) ?? new Word { Text = syn };
            if (s.Id == 0) _db.Words.Add(s);

            if (!_db.WordSynonyms.Any(ws => ws.WordId == w.Id && ws.SynonymId == s.Id))
            {
                _db.WordSynonyms.Add(new WordSynonym { Word = w, Synonym = s });
                _db.WordSynonyms.Add(new WordSynonym { Word = s, Synonym = w });
            }
        }

        _db.SaveChanges();
    }

    public IReadOnlyCollection<string> GetSynonyms(string word)
    {
        var w = _db.Words
            .Include(x => x.Synonyms)
            .ThenInclude(ws => ws.Synonym)
            .SingleOrDefault(x => x.Text == word);

        return w?.Synonyms.Select(ws => ws.Synonym!.Text).ToArray()
               ?? Array.Empty<string>();
    }

    public IReadOnlyCollection<string> GetAllWords()
    {
        return _db.Words.Select(x => x.Text).OrderBy(x => x).ToArray();
    }
}
