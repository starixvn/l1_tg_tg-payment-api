using PaymentAPI.Infrastructure.Services.Logging.Models;

namespace PaymentAPI.Infrastructure.Services.Logging;

public interface IApiLoggingService
{
    Task LogApiCallAsync(ApiLog log);
    Task<IEnumerable<ApiLog>> GetLogsAsync(DateTime? startDate = null, DateTime? endDate = null, string? path = null, int? statusCode = null);
    Task<ApiLog?> GetLogByIdAsync(string id);
} 