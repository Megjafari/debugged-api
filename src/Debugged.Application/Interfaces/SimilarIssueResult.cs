using Debugged.Domain.Entities;

namespace Debugged.Application.Interfaces;

// Return type for FindSimilarResolvedAsync — pairs the issue with its computed match score
// so the score (calculated in SQL) survives the trip back to the handler.
public record SimilarIssueResult(Issue Issue, int MatchScore);