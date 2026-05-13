using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlashCardz.API.Models;

public class Card
{
    [BsonElement("front")]
    public string Front { get; set; } = string.Empty;

    [BsonElement("back")]
    public string Back { get; set; } = string.Empty;
}

public class Deck
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("cards")]
    public List<Card> Cards { get; set; } = new();
}