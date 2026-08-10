# Take-home — Document Control Register

A small, self-contained .NET task. Time-box **~4–6 hours**. No external services, database or
credentials are needed — everything runs locally.

**Scope over completeness.** You are *not* expected to finish everything, and you shouldn't polish
beyond the time-box. We would much rather see a small, correct, well-tested core than a broad but
shaky one. Get the state machine and the audit trail right and tested first; if you run out of time,
just **write down what you'd do next and why** in your notes — that reasoning counts as much as code.

---

## In one sentence

You build a small C# library that manages **controlled documents** (like SOPs or policies) through
their lifecycle — and keeps a complete, tamper-proof **history of every change**. No UI, no real
API: just the logic and tests that prove it behaves.

## Worked example — what your library does

A document (say `SOP-001`) moves through states, and **every change is recorded in an audit trail**:

| Step | Action | Resulting state | Audit trail after |
|---|---|---|---|
| 1 | Alice creates `SOP-001` v1 "Incident Reporting Procedure" | **Draft** | `Created by alice` |
| 2 | Alice submits it for review | **InReview** | `… + Draft→InReview by alice` |
| 3 | Bob approves it | **Approved** | `… + InReview→Approved by bob` |
| 4 | Bob publishes it | **Published** | `… + Approved→Published by bob` |
| 5 | Someone tries to publish `SOP-002` straight from Draft | **rejected** | *(illegal transition — no change, no audit)* |
| 6 | `SOP-001` v2 is created and taken to Published | v2 **Published**, v1 → **Superseded** | *(both audited; only ONE Published at a time)* |
| 7 | Alice "deletes" `SOP-001` v1 | v1 **Archived** *(not a row deletion)* | `… + Archived by alice` — and it took a **two-step confirm** |
| 8 | Ask for `SOP-001`'s history | — | the **full ordered list**, nothing missing |

**Your job:** implement the register so these operations behave exactly like this — illegal
transitions are rejected, nothing changes a document without leaving an audit entry, a new
published version supersedes the old, nothing is ever hard-deleted — and write tests that prove it.

## The rules (precise version)

1. **Legal transitions only.** The allowed moves are:
   `Draft → InReview → Approved → Published`, `Published → Superseded`, and (from most states)
   `→ Archived`. Anything else (e.g. `Draft → Published`) must be **rejected** with a clear error.
2. **Immutable, complete audit trail.** Every transition **and** every metadata edit appends an
   audit entry (who, when, from→to, reason). Audit entries are **never updated or deleted**, and
   there must be **no path** to change a document without producing one.
3. **One Published per document number.** Publishing a new version supersedes the currently
   published one (old → `Superseded`). At most one `Published` version per number at any time.
4. **Soft-delete only.** "Delete" = `Archived` with an audit entry. No hard delete, ever.
5. **Two-step confirmation for the irreversible step** (archive): it must take an explicit confirm
   (a token, a second call — your design), not a single fire-and-forget call.
6. **A judgment call to make and justify:** when a document is superseded, what should happen to
   any *pending reading tasks* on the old version — carry them to the new version, close them, or
   reopen them? Pick one, implement it (even a simple model), and **explain why** in your notes.
   There is no single right answer; we want the reasoning.

## Deliverables

- This solution, with **EF Core against SQLite** for persistence (runs anywhere).
- The state-machine + audit logic as a **testable core** — `now` and `actor` are passed in, not
  read globally, so the audit records who/when deterministically.
- **Unit tests** covering the rules above and their edge cases.
- **Design notes (English)** — key design decisions, your reading-tasks rule and why, trade-offs,
  and what you'd do with more time. Put them in your pull-request description or a short `NOTES.md`.
- Optional: a tiny CLI or minimal API to demo. Not required; tests are read first.

## What we look for

- A state machine that rejects illegal transitions.
- An audit trail that is genuinely append-only and **complete** — nothing slips through unaudited.
- The supersede invariant (one Published) and the reading-tasks judgment.
- Two-step destructive action + soft-delete discipline.
- Clean, readable C#; EF Core used sensibly; reasoning in your notes. We care as much about *why*
  as *what*.

---

## Project layout

```
DocumentControl.sln
  src/DocumentControl.Core     domain types + IDocumentRegister — implement this
  tests/DocumentControl.Tests  xUnit; green smoke tests + a checklist of cases to add
```

The types in `DocumentControl.Core` are a **starting shape**, not a spec — adjust them (including
`IDocumentRegister`, e.g. to model the two-step archive) as your design needs.

## Running

```bash
dotnet test        # green from a clean clone; add your own tests here
dotnet build
```

Requires the .NET 8 SDK.

## How to submit

1. Click **"Use this template" → "Create a new repository"** to make your own copy. (This keeps
   your work separate — please don't fork or open a pull request against this repository.)
2. Implement the task in your copy, committing as you go.
3. Put your **design notes** in the repository's `README` / a short `NOTES.md`, or in the message
   you send back.
4. Send it back:
   - if your repository is **public**, reply with the link; or
   - if it's **private**, invite the reviewer (the GitHub username you were given) as a
     collaborator, or send a `.zip`.

No deadline pressure inside the task itself — spend the time-box, and tell us what you'd do with more.
