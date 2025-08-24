using Microsoft.AspNetCore.Mvc;
using Thesaurus.Api.Models;
using Thesaurus.core.Interfaces;

namespace Thesaurus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThesaurusController : ControllerBase
{
    private readonly IThesaurus _thesaurus;

    // Constructor to inject the IThesaurus dependency  
    public ThesaurusController(IThesaurus thesaurus) => _thesaurus = thesaurus;

    // Endpoint to add a new word with its synonyms  
    [HttpPost]
    public IActionResult AddWord([FromBody] AddWordRequest request)
    {
        // Validate the request to ensure the word is not null or empty  
        if (string.IsNullOrWhiteSpace(request?.Word))
            return BadRequest("Word is required.");

        // Add the word and its synonyms to the thesaurus  
        _thesaurus.AddWord(request.Word, request.Synonyms ?? Enumerable.Empty<string>());
        return Ok(new { success = true });
    }

    // Endpoint to retrieve synonyms for a specific word  
    [HttpGet("{word}")]
    public ActionResult<IReadOnlyCollection<string>> GetSynonyms(string word) =>
        Ok(_thesaurus.GetSynonyms(word));

    // Endpoint to retrieve all words in the thesaurus  
    [HttpGet]
    public ActionResult<IReadOnlyCollection<string>> GetAllWords() =>
        Ok(_thesaurus.GetAllWords());
}
