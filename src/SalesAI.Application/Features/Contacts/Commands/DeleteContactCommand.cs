using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Contacts.Commands;

public record DeleteContactCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _context.Contacts.FindAsync(new object[] { request.Id }, cancellationToken);

        if (contact == null)
        {
            return Result<bool>.Failure("Contact not found.");
        }

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
