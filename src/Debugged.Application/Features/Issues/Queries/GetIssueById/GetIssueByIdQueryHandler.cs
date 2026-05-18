using AutoMapper;
using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Issues;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Issues.Queries.GetIssueById;

public class GetIssueByIdQueryHandler : IRequestHandler<GetIssueByIdQuery, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IMapper _mapper;

    public GetIssueByIdQueryHandler(IIssueRepository issueRepository, IMapper mapper)
    {
        _issueRepository = issueRepository;
        _mapper = mapper;
    }

    public async Task<IssueDto> Handle(GetIssueByIdQuery request, CancellationToken cancellationToken)
    {
        // Eager-load tags — detail view needs them for the response DTO.
        var issue = await _issueRepository.GetByIdWithTagsAsync(request.Id, cancellationToken);
        if (issue is null)
        {
            throw new NotFoundException(nameof(Issue), request.Id);
        }

        return _mapper.Map<IssueDto>(issue);
    }
}