using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PaymentAPI.Infrastructure.Services.Logging.Models;
using PaymentAPI.Infrastructure.Services.Logging.Configuration;

namespace PaymentAPI.Infrastructure.Services.Logging.Middleware;

public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiLoggingService _loggingService;
    private readonly ApiLoggingOptions _options;
    private static readonly HashSet<string> _excludedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/octet-stream",
        "image/",
        "video/",
        "audio/",
        "application/pdf",
        "application/zip"
    };

    public ApiLoggingMiddleware(
        RequestDelegate next,
        IApiLoggingService loggingService,
        IOptions<ApiLoggingOptions> options)
    {
        _next = next;
        _loggingService = loggingService;
        _options = options.Value;
    }

    private static bool ShouldSkipContentType(HttpContext context)
    {
        var contentType = context.Request.ContentType;
        if (string.IsNullOrEmpty(contentType)) return false;

        return _excludedContentTypes.Any(excluded => contentType.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<string?> ReadBodyAsync(Stream stream, bool leaveOpen = true)
    {
        if (stream.Length == 0) return null;
        if (stream.Length > _options.MaxBodyLength) return "[Content too large to log]";

        using var reader = new StreamReader(
            stream,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: leaveOpen);

        return await reader.ReadToEndAsync();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if we should log this request based on configuration
        if (!_options.ShouldLogRequest(context) || ShouldSkipContentType(context))
        {
            await _next(context);
            return;
        }

        var log = new ApiLog
        {
            Path = context.Request.Path,
            Method = context.Request.Method,
            IpAddress = context.Connection.RemoteIpAddress?.ToString()
        };

        // Capture configured headers only
        foreach (var header in context.Request.Headers.Where(h => _options.ImportantHeaders.Contains(h.Key)))
        {
            log.Headers[header.Key] = header.Value.ToString();
        }

        // Handle request body
        var originalRequestBody = context.Request.Body;
        using var requestBodyStream = new MemoryStream();
        await originalRequestBody.CopyToAsync(requestBodyStream);
        requestBodyStream.Position = 0;

        log.RequestBody = await ReadBodyAsync(requestBodyStream);
        requestBodyStream.Position = 0;
        context.Request.Body = requestBodyStream;

        var originalResponseBody = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);

            stopwatch.Stop();
            log.ExecutionTime = stopwatch.ElapsedMilliseconds;
            log.StatusCode = context.Response.StatusCode;

            // Only log response for non-success status codes or if it's small enough
            if (context.Response.StatusCode >= 400 || responseBody.Length <= _options.MaxBodyLength)
            {
                responseBody.Position = 0;
                log.ResponseBody = await ReadBodyAsync(responseBody);
            }

            // Copy the response to the original stream
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            log.ExecutionTime = stopwatch.ElapsedMilliseconds;
            log.StatusCode = 500;
            log.ErrorMessage = ex.Message;
            throw;
        }
        finally
        {
            context.Request.Body = originalRequestBody;
            
            // Fire and forget logging to avoid impacting response time
            _ = Task.Run(async () => 
            {
                try 
                {
                    await _loggingService.LogApiCallAsync(log);
                }
                catch
                {
                    // Suppress logging errors to avoid impacting the main request
                }
            });
        }
    }
} 