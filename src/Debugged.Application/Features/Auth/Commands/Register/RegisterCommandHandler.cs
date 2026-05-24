using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Auth;
using Debugged.Application.Interfaces;
using MediatR;

namespace Debugged.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, errors) = await _identityService.RegisterAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // Identity rejects the registration (duplicate email, password policy backstop, etc.)
        // Surface errors as our standard ValidationException so the middleware returns a clean 400.
        if (!succeeded)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Registration"] = errors
            });
        }

        // New users always get the default "User" role — admins are created via seeding.
        var roles = new[] { "User" };
        var token = _tokenService.CreateToken(userId, request.Email, roles);

        return new AuthResponseDto(
            token.Token,
            token.ExpiresAt,
            userId,
            request.Email,
            roles);
    }
}