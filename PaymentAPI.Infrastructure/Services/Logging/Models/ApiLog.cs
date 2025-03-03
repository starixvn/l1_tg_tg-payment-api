using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaymentAPI.Infrastructure.Services.Logging.Models;

public class ApiLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Path { get; set; } = null!;
    
    public string Method { get; set; } = null!;
    
    public string? RequestBody { get; set; }
    
    public string? ResponseBody { get; set; }
    
    public int StatusCode { get; set; }
    
    public Dictionary<string, string> Headers { get; set; } = new();
    
    public string? IpAddress { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public long ExecutionTime { get; set; } // in milliseconds
    
    public string? ErrorMessage { get; set; }
} 