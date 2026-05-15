using FluentValidation;

namespace Debugged.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be 200 characters or fewer.");

        RuleFor(c => c.Description)
            .MaximumLength(2000).WithMessage("Description must be 2000 characters or fewer.");
    }
}