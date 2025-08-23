using Microsoft.AspNetCore.Mvc;
using Thesaurus.Core;
using Thesaurus.Api.Models;
using Thesaurus.core;

namespace Thesaurus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThesaurusController : ControllerBase
{
    private readonly IThesaurus _thesaurus;

    public ThesaurusController(IThesaurus thesaurus)
    {
        _thesaurus = thesaurus;
    }

    [HttpPost]
    public IActionResult AddWord([FromBody] AddWordRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Word))
            return BadRequest("Word is required.");

        _thesaurus.AddWord(request.Word, request.Synonyms ?? Enumerable.Empty<string>());
        return Ok(new { success = true });
    }

    [HttpGet("{word}")]
    public ActionResult<IReadOnlyCollection<string>> GetSynonyms(string word)
        => Ok(_thesaurus.GetSynonyms(word));

    [HttpGet]
    public ActionResult<IReadOnlyCollection<string>> GetAllWords()
        => Ok(_thesaurus.GetAllWords());
}
