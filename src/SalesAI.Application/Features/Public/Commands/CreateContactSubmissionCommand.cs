using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Public.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Public.Commands;

public record CreateContactSubmissionCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Company,
    string InquiryType,
    string Subject,
    string Message) : IRequest<Result<ContactSubmissionResponse>>;

public class CreateContactSubmissionCommandValidator : AbstractValidator<CreateContactSubmissionCommand>
{
    public CreateContactSubmissionCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(v => v.Phone).MaximumLength(50);
        RuleFor(v => v.Company).MaximumLength(200);
        RuleFor(v => v.InquiryType).NotEmpty().IsEnumName(typeof(ContactInquiryType), caseSensitive: false);
        RuleFor(v => v.Subject).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Message).NotEmpty().MaximumLength(5000);
    }
}

public class CreateContactSubmissionCommandHandler : IRequestHandler<CreateContactSubmissionCommand, Result<ContactSubmissionResponse>>
{
    private readonly IApplicationDbContext _context;

    public CreateContactSubmissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ContactSubmissionResponse>> Handle(CreateContactSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ContactInquiryType>(request.InquiryType, true, out var inquiryType))
        {
            return Result<ContactSubmissionResponse>.Failure("Invalid inquiry type.");
        }

        var submission = new ContactSubmission
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            Company = request.Company,
            InquiryType = inquiryType,
            Subject = request.Subject,
            Message = request.Message,
            IsProcessed = false
        };

        _context.ContactSubmissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ContactSubmissionResponse>.Success(
            new ContactSubmissionResponse(submission.Id, "Thank you for reaching out! We'll get back to you within 24 hours."));
    }
}
