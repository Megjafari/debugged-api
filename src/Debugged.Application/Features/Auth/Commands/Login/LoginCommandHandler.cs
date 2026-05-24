using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Auth;
using Debugged.Application.Interfaces;
using MediatR;

namespace Debugged.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // Generic "Invalid credentials" message — never reveal whether the email exists or
        // the password was wrong. Prevents account enumeration attacks.
        if (!result.Succeeded)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Credentials"] = new[] { "Invalid email or password." }
            });
        }

        var token = _tokenService.CreateToken(result.UserId, result.Email, result.Roles);

        return new AuthResponseDto(
            token.Token,
            token.ExpiresAt,
            result.UserId,
            result.Email,
            result.Roles);
    }
}