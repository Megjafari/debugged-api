using FluentValidation;

namespace Debugged.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        // Don't reuse RegisterCommandValidator's password rules here — for login we only check
        // that values are present. Strength is irrelevant; the password either matches or doesn't.
        // Leaking "weak password" on login would also be an info leak about account state.
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}