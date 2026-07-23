using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class Appointment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string PatientId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled";
}
