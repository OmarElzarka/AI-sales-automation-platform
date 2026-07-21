using Microsoft.Extensions.Logging;
using SalesAI.Application.Common.Interfaces;

namespace SalesAI.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public class CacheService : ICacheService
{
    // In-memory stub for MVP — swap to Redis in Phase 2
    private readonly Dictionary<string, (object Value, DateTime? Expiry)> _cache = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiry.HasValue && entry.Expiry.Value < DateTime.UtcNow)
            {
                _cache.Remove(key);
                return Task.FromResult(default(T));
            }
            return Task.FromResult((T?)entry.Value);
        }
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var expiryTime = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : (DateTime?)null;
        _cache[key] = (value!, expiryTime);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}

public class EmailService : IEmailService
{
    private readonly Microsoft.Extensions.Logging.ILogger<EmailService> _logger;

    public EmailService(Microsoft.Extensions.Logging.ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        // Log-only stub for MVP — swap to SendGrid/SMTP in Phase 2
        _logger.LogInformation("📧 Email sent to {To} | Subject: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}
