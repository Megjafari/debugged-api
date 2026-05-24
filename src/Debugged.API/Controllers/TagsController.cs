using Debugged.Application.DTOs.Tags;
using Debugged.Application.Features.Tags.Commands.CreateTag;
using Debugged.Application.Features.Tags.Commands.DeleteTag;
using Debugged.Application.Features.Tags.Queries.GetAllTags;
using Debugged.Application.Features.Tags.Queries.GetTagById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Debugged.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns all tags, sorted alphabetically.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllTagsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns a single tag by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTagByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a curated tag — used to pre-seed the taxonomy.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagDto>> Create(
        [FromBody] CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Removes a tag. Linked issues lose the tag via cascade delete on IssueTag rows.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTagCommand(id), cancellationToken);
        return NoContent();
    }
}