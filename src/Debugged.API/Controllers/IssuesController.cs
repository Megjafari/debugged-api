using Debugged.API.Contracts.Issues;
using Debugged.Application.DTOs.Issues;
using Debugged.Application.Features.Issues.Commands.CreateIssue;
using Debugged.Application.Features.Issues.Commands.DeleteIssue;
using Debugged.Application.Features.Issues.Commands.UpdateIssue;
using Debugged.Application.Features.Issues.Queries.GetAllIssuesByProject;
using Debugged.Application.Features.Issues.Queries.GetIssueById;
using Debugged.Application.Features.Issues.Queries.GetSimilarIssues;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Debugged.API.Controllers;

[ApiController]
[Produces("application/json")]
public class IssuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IssuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Listing issues is always scoped to a project — reflects domain hierarchy in the URL.
    [HttpGet("api/projects/{projectId:guid}/issues")]
    [ProducesResponseType(typeof(IReadOnlyList<IssueListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<IssueListDto>>> GetByProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllIssuesByProjectQuery(projectId), cancellationToken);
        return Ok(result);
    }

    // Detail view, similar lookup, update and delete operate on a single issue —
    // flat /api/issues/{id} route keeps these clean (no projectId duplication required).
    [HttpGet("api/issues/{id:guid}")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IssueDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetIssueByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    // Star feature: similar resolved issues for a given source issue.
    [HttpGet("api/issues/{id:guid}/similar")]
    [ProducesResponseType(typeof(IReadOnlyList<SimilarIssueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<SimilarIssueDto>>> GetSimilar(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSimilarIssuesQuery(id), cancellationToken);
        return Ok(result);
    }

    // Create lives under the project so projectId in the URL stays consistent with GetByProject.
    [HttpPost("api/projects/{projectId:guid}/issues")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IssueDto>> Create(
        Guid projectId,
        [FromBody] CreateIssueRequest body,
        CancellationToken cancellationToken)
    {
        // Bind route's projectId into the command — don't trust a body-only value.
        // This prevents URL/body mismatch and a class of mass-assignment mistakes.
        var command = new CreateIssueCommand(
            body.Title,
            body.Description,
            body.ErrorMessage,
            body.Solution,
            body.Priority,
            projectId,
            body.Tags);

        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("api/issues/{id:guid}")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IssueDto>> Update(
        Guid id,
        [FromBody] UpdateIssueRequest body,
        CancellationToken cancellationToken)
    {
        var command = new UpdateIssueCommand(
            id,
            body.Title,
            body.Description,
            body.ErrorMessage,
            body.Solution,
            body.Status,
            body.Priority,
            body.Tags);

        var updated = await _mediator.Send(command, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("api/issues/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteIssueCommand(id), cancellationToken);
        return NoContent();
    }
}