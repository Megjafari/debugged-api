using MediatR;

namespace Debugged.Application.Features.Tags.Commands.DeleteTag;

// Hard delete — unlike Issues, tags don't carry knowledge worth archiving.
// Removing a tag cascades to IssueTag rows (configured in IssueTagConfiguration),
// which means linked issues lose the tag but stay intact.
public record DeleteTagCommand(Guid Id) : IRequest;