using Debugged.Application.DTOs.Tags;
using MediatR;

namespace Debugged.Application.Features.Tags.Commands.CreateTag;

// Admin-only creation of a tag — used to seed the taxonomy ahead of time.
// Note: tags also get created on-the-fly when issues are tagged, so this endpoint is for
// curated/blessed tags that should exist even before any issue uses them.
public record CreateTagCommand(string Name) : IRequest<TagDto>;