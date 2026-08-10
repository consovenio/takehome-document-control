using DocumentControl.Core;
using Xunit;

namespace DocumentControl.Tests;

/// <summary>
/// A green starting point so `dotnet test` passes from a clean clone. Replace and expand this with
/// real tests of your register. Cases worth covering (not exhaustive) - these mirror the worked
/// example in the README:
///
///   - Legal path: Draft -> InReview -> Approved -> Published works and audits each step.
///   - Illegal transition is rejected (e.g. Draft -> Published, skipping review).
///   - Publishing v2 supersedes v1: only one Published per document number at a time.
///   - Every change leaves an audit entry; there is NO path to a new state without one.
///   - Audit is append-only: existing entries are never modified or removed.
///   - Archive is a soft-delete (state, not row deletion) and requires the two-step confirm.
///   - now/actor are inputs, so the audit records who and when deterministically.
/// </summary>
public sealed class SmokeTests
{
    [Fact]
    public void A_new_document_version_starts_as_a_draft()
    {
        var doc = new ControlledDocument
        {
            DocumentNumber = "SOP-001",
            Version = 1,
            Title = "Incident Reporting Procedure",
            Owner = "alice",
        };

        Assert.Equal(DocumentState.Draft, doc.State);
    }

    [Fact]
    public void An_audit_entry_records_a_transition()
    {
        var entry = new AuditEntry
        {
            DocumentNumber = "SOP-001",
            Version = 1,
            At = DateTimeOffset.UnixEpoch,
            Actor = "bob",
            Action = "Transition",
            FromState = DocumentState.Approved,
            ToState = DocumentState.Published,
            Reason = "Sign-off complete",
        };

        Assert.Equal(DocumentState.Approved, entry.FromState);
        Assert.Equal(DocumentState.Published, entry.ToState);
    }
}
