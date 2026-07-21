using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Public.Models;
using SalesAI.Domain.Entities;

namespace SalesAI.Application.Features.Public.Commands;

public record SubscribeNewsletterCommand(string Email) : IRequest<Result<NewsletterResponse>>;

public class SubscribeNewsletterCommandValidator : AbstractValidator<SubscribeNewsletterCommand>
{
    public SubscribeNewsletterCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress().MaximumLength(255);
    }
}

public class SubscribeNewsletterCommandHandler : IRequestHandler<SubscribeNewsletterCommand, Result<NewsletterResponse>>
{
    private readonly IApplicationDbContext _context;

    public SubscribeNewsletterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<NewsletterResponse>> Handle(SubscribeNewsletterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLowerInvariant();

        // Check for existing subscription
        var existing = await _context.NewsletterSubscriptions
            .FirstOrDefaultAsync(n => n.Email == normalizedEmail, cancellationToken);

        if (existing != null)
        {
            if (existing.IsActive)
            {
                return Result<NewsletterResponse>.Success(
                    new NewsletterResponse(normalizedEmail, "You're already subscribed to our newsletter!"));
            }

            // Reactivate
            existing.IsActive = true;
            existing.SubscribedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<NewsletterResponse>.Success(
                new NewsletterResponse(normalizedEmail, "Welcome back! You've been re-subscribed to our newsletter."));
        }

        var subscription = new NewsletterSubscription
        {
            Email = normalizedEmail,
            SubscribedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.NewsletterSubscriptions.Add(subscription);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<NewsletterResponse>.Success(
            new NewsletterResponse(normalizedEmail, "Successfully subscribed! You'll receive our latest updates and insights."));
    }
}
