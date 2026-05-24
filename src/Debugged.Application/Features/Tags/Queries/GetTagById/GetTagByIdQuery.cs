using Debugged.Application.DTOs.Tags;
using MediatR;

namespace Debugged.Application.Features.Tags.Queries.GetTagById;

public record GetTagByIdQuery(Guid Id) : IRequest<TagDto>;