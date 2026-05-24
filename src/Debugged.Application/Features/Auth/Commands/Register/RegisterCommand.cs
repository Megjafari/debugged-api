using Debugged.Application.DTOs.Auth;
using MediatR;

namespace Debugged.Application.Features.Auth.Commands.Register;

// New-user registration. Assigns the "User" role by default — admins are created via seeding.
public record RegisterCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;