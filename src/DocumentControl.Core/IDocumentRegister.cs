namespace DocumentControl.Core;

/// <summary>
/// The register and its rules - the heart of the task. Implement this (EF Core against SQLite is
/// expected, but the state-machine and audit logic should be testable without a database).
///
/// Pass <c>now</c> and <c>actor</c> in rather than reading them globally, so the behaviour - and
/// the audit trail - is testable without faking time or identity. The rules to honour are in the
/// README:
///   1. Only legal transitions   2. Immutable, complete audit trail
///   3. One Published per number (publishing a new version supersedes the old)
///   4. Soft-delete only (Archive), with two-step confirmation for that irreversible step.
///
/// This interface is a STARTING SHAPE - change signatures as your design needs (e.g. to model the
/// two-step archive confirmation).
/// </summary>
public interface IDocumentRegister
{
    /// <summary>Create a new document version in <see cref="DocumentState.Draft"/>. Writes an audit entry.</summary>
    Task<ControlledDocument> CreateDraftAsync(
        string documentNumber, int version, string title, string owner, string actor,
        DateTimeOffset now, CancellationToken ct);

    /// <summary>
    /// Move a document version to a new state. Must reject an illegal transition, write an audit
    /// entry, and - when publishing - supersede the previously published version of the same number.
    /// </summary>
    Task TransitionAsync(
        string documentNumber, int version, DocumentState to, string actor, string? reason,
        DateTimeOffset now, CancellationToken ct);

    /// <summary>Soft-retire a document version (never a row deletion). Must be a two-step action -
    /// see the README - and must write an audit entry.</summary>
    Task ArchiveAsync(
        string documentNumber, int version, string actor, string? reason,
        DateTimeOffset now, CancellationToken ct);

    /// <summary>The full, ordered history for a document number. Nothing may be missing.</summary>
    Task<IReadOnlyList<AuditEntry>> GetAuditTrailAsync(string documentNumber, CancellationToken ct);

    /// <summary>The currently published version of a document number, if any.</summary>
    Task<ControlledDocument?> GetPublishedAsync(string documentNumber, CancellationToken ct);
}
