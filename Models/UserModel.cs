using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace mongoAuth.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; } = null!;

        public string Password { get; set; }

        public string Role { get; set; } = null!;

        public List<string> Projects { get; set; } = null!;

        public string Level { get; set; } = null!;
  
    }
}
