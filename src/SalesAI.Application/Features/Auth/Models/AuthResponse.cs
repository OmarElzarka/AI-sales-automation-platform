namespace SalesAI.Application.Features.Auth.Models;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Role,
    string? AvatarUrl);
