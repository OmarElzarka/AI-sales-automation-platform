using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Dashboard.Models;

public record ActivityDto(
    Guid Id,
    string Type,
    string Title,
    string? Description,
    DateTime CreatedAt,
    string? PerformedBy,
    Guid? LeadId,
    string? LeadName,
    Guid? DealId,
    string? DealTitle);
