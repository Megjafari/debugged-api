using FluentValidation;

namespace Debugged.Application.Features.Issues.Commands.UpdateIssue;

public class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.ErrorMessage)
            .MaximumLength(2000)
            .When(x => x.ErrorMessage is not null);

        RuleFor(x => x.Solution)
            .MaximumLength(5000)
            .When(x => x.Solution is not null);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.Tags)
            .NotNull();

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(50);

        // Business rule: resolving an issue requires a solution. The whole point of this
        // app is to capture *how* a bug was fixed — letting Resolved without Solution defeats it.
        RuleFor(x => x.Solution)
            .NotEmpty()
            .When(x => x.Status == Domain.Enums.IssueStatus.Resolved)
            .WithMessage("A solution is required when marking an issue as Resolved.");
    }
}