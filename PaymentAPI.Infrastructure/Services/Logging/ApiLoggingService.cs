using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PaymentAPI.Infrastructure.Services.Logging.Configuration;
using PaymentAPI.Infrastructure.Services.Logging.Models;

namespace PaymentAPI.Infrastructure.Services.Logging;

public class ApiLoggingService : IApiLoggingService
{
    private readonly IMongoCollection<ApiLog> _logs;
    private static readonly string IndexName = "payment_log_query_index";
    private readonly ILogger<ApiLoggingService> _logger;

    public ApiLoggingService(
        IMongoDatabase database,
        IOptions<ApiLoggingOptions> settings,
        ILogger<ApiLoggingService> logger)
    {
        _logs = database.GetCollection<ApiLog>(
            settings.Value.LogsCollectionName);
        _logger = logger;

        EnsureIndexesCreated();
    }

    private void EnsureIndexesCreated()
    {
        try
        {
            // Check if our index already exists
            var indexes = _logs.Indexes.List().ToList();
            if (indexes.Any(idx => idx["name"] == IndexName))
            {
                return;
            }

            // Create indexes for better query performance
            var indexKeysDefinition = Builders<ApiLog>.IndexKeys
                .Ascending(x => x.Timestamp)
                .Ascending(x => x.Path)
                .Ascending(x => x.StatusCode);

            var indexOptions = new CreateIndexOptions
            {
                Background = true,
                Name = IndexName
            };

            var indexModel = new CreateIndexModel<ApiLog>(indexKeysDefinition, indexOptions);
            _logs.Indexes.CreateOne(indexModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create index for ApiLogs collection");
        }
    }

    public async Task LogApiCallAsync(ApiLog log)
    {
        await _logs.InsertOneAsync(log);
    }

    public async Task<IEnumerable<ApiLog>> GetLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? path = null,
        int? statusCode = null)
    {
        var builder = Builders<ApiLog>.Filter;
        var filter = builder.Empty;

        if (startDate.HasValue)
            filter &= builder.Gte(x => x.Timestamp, startDate.Value);
        
        if (endDate.HasValue)
            filter &= builder.Lte(x => x.Timestamp, endDate.Value);
        
        if (!string.IsNullOrEmpty(path))
            filter &= builder.Eq(x => x.Path, path);
        
        if (statusCode.HasValue)
            filter &= builder.Eq(x => x.StatusCode, statusCode.Value);

        return await _logs.Find(filter)
            .Sort(Builders<ApiLog>.Sort.Descending(x => x.Timestamp))
            .ToListAsync();
    }

    public async Task<ApiLog?> GetLogByIdAsync(string id)
    {
        return await _logs.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
} 