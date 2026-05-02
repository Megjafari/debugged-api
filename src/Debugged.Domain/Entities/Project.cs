using Debugged.Domain.Common;

namespace Debugged.Domain.Entities;

// A Project groups related Issues together. One developer might
// have several projects (e.g. "Personal Blog", "Work API", "Side Project")
// and each issue belongs to exactly one project.
public class Project : BaseEntity
{
    // Required field — every project must have a name.
    // Default to empty string instead of null to satisfy nullable
    // reference types without making the property itself nullable.
    public string Name { get; set; } = string.Empty;

    // Optional — some projects don't need a description.
    public string? Description { get; set; }

    // Navigation property — EF Core uses this to load related issues.
    // Initialized to an empty list so we can safely call .Add() on a
    // brand-new Project before EF has materialized it from the database.
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}