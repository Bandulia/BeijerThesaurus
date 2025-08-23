namespace Thesaurus.Api.Models;

public class AddWordRequest
{
    public string? Word { get; set; }
    public IEnumerable<string>? Synonyms { get; set; }
}
