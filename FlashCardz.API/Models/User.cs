using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlashCardz.API.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("totalDecks")]
    public int TotalDecks { get; set; } = 0;

    [BsonElement("learningStreak")]
    public int LearningStreak { get; set; } = 0;
}