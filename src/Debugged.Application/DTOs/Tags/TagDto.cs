namespace Debugged.Application.DTOs.Tags;

// Tag taxonomy DTO — used for the admin-facing /api/tags endpoints.
// Issues expose tags as plain strings (see IssueDto.Tags); this DTO is for managing the tag table itself.
public record TagDto(
    Guid Id,
    string Name,
    DateTime CreatedAt
);