using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentAPI.Infrastructure.Services.Logging.Configuration;

public class ApiLoggingOptions
{
    public bool LogAllRequests { get; set; } = false;
    public string LogsCollectionName { get; set; } = "payment_api_logs";
    public List<string> IncludePaths { get; set; } = new();
    public List<string> ExcludePaths { get; set; } = new();
    public List<string> IncludeControllers { get; set; } = new();
    public int MaxBodyLength { get; set; } = 32 * 1024; // 32KB default
    public string[] ImportantHeaders { get; set; } = new[]
    {
        "User-Agent",
        "Accept",
        "Content-Type",
        "X-Forwarded-For"
    };

    internal bool ShouldLogRequest(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var controller = context.GetRouteValue("controller")?.ToString()?.ToLower();

        // If specific controllers are configured, only log those
        if (IncludeControllers.Any())
        {
            return controller != null && 
                   IncludeControllers.Any(c => c.Equals(controller, StringComparison.OrdinalIgnoreCase));
        }

        // If no include paths are specified and LogAllRequests is false, don't log
        if (!LogAllRequests && !IncludePaths.Any())
        {
            return false;
        }

        // Check exclude paths first
        if (ExcludePaths.Any(excludePath => 
            path.StartsWith(excludePath.ToLower(), StringComparison.OrdinalIgnoreCase) ||
            Regex.IsMatch(path, excludePath, RegexOptions.IgnoreCase)))
        {
            return false;
        }

        // If include paths are specified, check them
        if (IncludePaths.Any())
        {
            return IncludePaths.Any(includePath =>
                path.StartsWith(includePath.ToLower(), StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(path, includePath, RegexOptions.IgnoreCase));
        }

        // If we get here and LogAllRequests is true, log the request
        return LogAllRequests;
    }
} 