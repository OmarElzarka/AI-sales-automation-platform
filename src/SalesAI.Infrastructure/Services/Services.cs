using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using SalesAI.Application.Common.Interfaces;

namespace SalesAI.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public class CacheService : ICacheService
{
    private readonly Microsoft.Extensions.Caching.Distributed.IDistributedCache _cache;

    public CacheService(Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var cachedData = await _cache.GetStringAsync(key, ct);
        if (string.IsNullOrEmpty(cachedData))
        {
            return default;
        }

        return System.Text.Json.JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var serializedData = System.Text.Json.JsonSerializer.Serialize(value);
        var options = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions();
        
        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }

        await _cache.SetStringAsync(key, serializedData, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
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
