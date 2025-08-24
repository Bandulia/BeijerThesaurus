using System.Net.Http;
using System.Net.Http.Json;

namespace Thesaurus.Wpf.Services;

public sealed class ThesaurusApi
{
    private readonly HttpClient _http;

    // Constructor to initialize the HttpClient with the base URL
    public ThesaurusApi(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    // Adds a new word with its synonyms to the thesaurus
    public async Task AddWordAsync(string word, IEnumerable<string> synonyms, CancellationToken ct = default)
    {
        try
        {
            var payload = new AddWordRequest { Word = word, Synonyms = synonyms?.ToArray() ?? Array.Empty<string>() };
            await PostAsync("/api/thesaurus", payload, ct);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Failed to add the word to the thesaurus.", ex);
        }
    }

    // Retrieves synonyms for a given word
    public async Task<IReadOnlyList<string>> GetSynonymsAsync(string word, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<IReadOnlyList<string>>($"/api/thesaurus/{Uri.EscapeDataString(word)}", ct)
                   ?? Array.Empty<string>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to retrieve synonyms for the word '{word}'.", ex);
        }
    }

    // Retrieves all words in the thesaurus
    public async Task<IReadOnlyList<string>> GetAllWordsAsync(CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<IReadOnlyList<string>>("/api/thesaurus", ct)
                   ?? Array.Empty<string>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Failed to retrieve all words from the thesaurus.", ex);
        }
    }

    // Helper method to send POST requests with a payload
    private async Task PostAsync<T>(string url, T payload, CancellationToken ct)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync(url, payload, ct);
            resp.EnsureSuccessStatusCode(); // Ensure the response indicates success
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to send POST request to '{url}'.", ex);
        }
    }

    // Represents the payload for adding a word to the thesaurus
    public sealed class AddWordRequest
    {
        public string? Word { get; set; }
        public IEnumerable<string>? Synonyms { get; set; }
    }
}

