namespace DocumentControl.Core;

/// <summary>
/// The lifecycle state of a controlled document. Only defined transitions between these are
/// allowed (see the README) - anything else must be rejected.
/// </summary>
public enum DocumentState
{
    Draft = 0,
    InReview = 1,
    Approved = 2,
    Published = 3,

    /// <summary>Replaced by a newer published version.</summary>
    Superseded = 4,

    /// <summary>Soft-retired. This is the ONLY way a document "goes away" - never a row deletion.</summary>
    Archived = 5,
}
