using FlashCardz.API.Models;
using MongoDB.Driver;

namespace FlashCardz.API.Services;

public class DeckService
{
    private readonly IMongoCollection<Deck> _decks;

    public DeckService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _decks = database.GetCollection<Deck>("Decks");
    }

    public async Task<List<Deck>> GetByUserIdAsync(string userId) =>
        await _decks.Find(d => d.UserId == userId).ToListAsync();

    public async Task<Deck?> GetByIdAsync(string id) =>
        await _decks.Find(d => d.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Deck deck) =>
        await _decks.InsertOneAsync(deck);

    public async Task UpdateAsync(string id, Deck deck) =>
        await _decks.ReplaceOneAsync(d => d.Id == id, deck);

    public async Task DeleteAsync(string id) =>
        await _decks.DeleteOneAsync(d => d.Id == id);
}