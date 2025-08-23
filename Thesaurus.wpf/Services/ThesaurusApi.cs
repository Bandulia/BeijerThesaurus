using System.Net.Http;
using System.Net.Http.Json;

namespace Thesaurus.Wpf.Services;

public sealed class ThesaurusApi
{
    private readonly HttpClient _http;
    public ThesaurusApi(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public Task AddWordAsync(string word, IEnumerable<string> synonyms, CancellationToken ct = default)
    {
        var payload = new AddWordRequest { Word = word, Synonyms = synonyms?.ToArray() ?? Array.Empty<string>() };
        return PostAsync("/api/thesaurus", payload, ct);
    }

    public async Task<IReadOnlyList<string>> GetSynonymsAsync(string word, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<string>>($"/api/thesaurus/{Uri.EscapeDataString(word)}", ct)
           ?? Array.Empty<string>();

    public async Task<IReadOnlyList<string>> GetAllWordsAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<string>>("/api/thesaurus", ct)
           ?? Array.Empty<string>();

    private async Task PostAsync<T>(string url, T payload, CancellationToken ct)
    {
        var resp = await _http.PostAsJsonAsync(url, payload, ct);
        resp.EnsureSuccessStatusCode();
    }

    public sealed class AddWordRequest
    {
        public string? Word { get; set; }
        public IEnumerable<string>? Synonyms { get; set; }
    }
}

