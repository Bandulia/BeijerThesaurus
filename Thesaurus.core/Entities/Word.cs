using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thesaurus.core.Entities
{    public class Word
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public ICollection<WordSynonym> Synonyms { get; set; } = new List<WordSynonym>();
    }
}
