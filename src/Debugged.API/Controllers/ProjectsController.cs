using Debugged.Application.DTOs.Projects;
using Debugged.Application.Features.Projects.Commands.CreateProject;
using Debugged.Application.Features.Projects.Queries.GetAllProjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Debugged.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    // Controller only dispatches via MediatR — no business logic, no direct service calls.
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns all projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllProjectsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(command, cancellationToken);

        // 201 Created with Location header pointing to a future GetById endpoint.
        // We use nameof(GetAll) for now since GetById isn't implemented yet — will refactor when GetProjectByIdQuery is added.
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}