using FluentValidation;

namespace Debugged.Application.Features.Issues.Commands.CreateIssue;

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(5000);

        // ErrorMessage is optional, but if provided it must fit in the column.
        RuleFor(x => x.ErrorMessage)
            .MaximumLength(2000)
            .When(x => x.ErrorMessage is not null);

        RuleFor(x => x.Solution)
            .MaximumLength(5000)
            .When(x => x.Solution is not null);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        // Tags list itself is required (can be empty) but no individual tag can be blank or huge.
        RuleFor(x => x.Tags)
            .NotNull();

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(50);
    }
}