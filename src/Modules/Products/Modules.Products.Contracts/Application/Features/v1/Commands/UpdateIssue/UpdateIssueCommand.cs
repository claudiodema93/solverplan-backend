using FSH.Modules.Products.Contracts.Domain.Enums;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;

/// <summary>
/// Command to update an existing issue.
/// </summary>
/// <param name="Id">The unique identifier of the issue to update</param>
/// <param name="Title">The issue title</param>
/// <param name="Description">Detailed description of the issue</param>
/// <param name="Severity">The severity level of the issue</param>
/// <param name="Status">The current status of the issue</param>
/// <param name="ResolutionNotes">Optional notes about the issue resolution</param>
public sealed record UpdateIssueCommand(
    int Id,
    string Title,
    string Description,
    IssueSeverity Severity,
    IssueState Status,
    string? ResolutionNotes) : ICommand;
