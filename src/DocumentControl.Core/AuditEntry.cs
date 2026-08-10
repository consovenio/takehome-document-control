namespace DocumentControl.Core;

/// <summary>
/// One immutable line in a document's history. Audit entries are append-only: never updated,
/// never deleted. Every state transition and every metadata change must produce one - there must
/// be no way to change a document without leaving an audit entry behind.
/// </summary>
public sealed record AuditEntry
{
    public required string DocumentNumber { get; init; }

    public required int Version { get; init; }

    public required DateTimeOffset At { get; init; }

    public required string Actor { get; init; }

    /// <summary>What happened, e.g. "Created", "Transition", "Archived", "TitleChanged".</summary>
    public required string Action { get; init; }

    /// <summary>State before, for a transition; null for a create or a pure metadata change.</summary>
    public DocumentState? FromState { get; init; }

    /// <summary>State after, for a transition; null for a pure metadata change.</summary>
    public DocumentState? ToState { get; init; }

    public string? Reason { get; init; }
}
