using Debugged.Application.DTOs.Tags;
using MediatR;

namespace Debugged.Application.Features.Tags.Queries.GetAllTags;

// Empty record — no parameters needed, returns the full tag taxonomy sorted alphabetically.
public record GetAllTagsQuery : IRequest<IReadOnlyList<TagDto>>;