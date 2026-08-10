namespace DocumentControl.Core;

/// <summary>
/// A controlled document version. Identity is (<see cref="DocumentNumber"/>, <see cref="Version"/>):
/// "SOP-001" can have a v1, v2, ... and at most one of them may be <see cref="DocumentState.Published"/>
/// at any time.
///
/// This is a STARTING SHAPE, not a spec - add, rename or drop fields as your design needs. It is
/// here so the test project compiles and the vocabulary is shared; the rules live in your
/// <see cref="IDocumentRegister"/> implementation.
/// </summary>
public sealed record ControlledDocument
{
    public required string DocumentNumber { get; init; }

    public required int Version { get; init; }

    public required string Title { get; init; }

    public required string Owner { get; init; }

    public DocumentState State { get; init; } = DocumentState.Draft;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
