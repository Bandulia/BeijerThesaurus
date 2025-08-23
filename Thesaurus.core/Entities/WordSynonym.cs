using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thesaurus.core.Entities
{    public class WordSynonym
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public int SynonymId { get; set; }

        public Word? Word { get; set; }
        public Word? Synonym { get; set; }
    }
}
