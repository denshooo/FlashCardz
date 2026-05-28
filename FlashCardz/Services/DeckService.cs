using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace FlashCardz.Services;

public class DeckModel
{
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<CardModel> Cards { get; set; } = new();
}

public class CardModel
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
}

public class DeckService
{
    private readonly HttpClient _http;
#if DEBUG
    private const string BaseUrl = "http://10.0.2.2:5170/api/deck";
#else
    private const string BaseUrl = "https://bigdeckapi-production.up.railway.app/api/deck";
#endif

    public DeckService()
    {
        _http = new HttpClient();
    }

    private async Task SetAuthHeader()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (token != null)
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<DeckModel>> GetDecksAsync()
    {
        await SetAuthHeader();
        try
        {
            var decks = await _http.GetFromJsonAsync<List<DeckModel>>(BaseUrl);
            return decks ?? new List<DeckModel>();
        }
        catch { return new List<DeckModel>(); }
    }

    public async Task<(bool Success, DeckModel? Deck)> CreateDeckAsync(
        string title, List<CardModel>? cards = null)
    {
        await SetAuthHeader();
        try
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, new
            {
                title,
                cards = cards ?? new List<CardModel>()
            });
            if (response.IsSuccessStatusCode)
            {
                var deck = await response.Content.ReadFromJsonAsync<DeckModel>();
                return (true, deck);
            }
            return (false, null);
        }
        catch { return (false, null); }
    }

    public async Task<bool> DeleteDeckAsync(string id)
    {
        await SetAuthHeader();
        try
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateDeckAsync(string id, string title, List<CardModel> cards)
    {
        await SetAuthHeader();
        try
        {
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/{id}", new { title, cards });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}