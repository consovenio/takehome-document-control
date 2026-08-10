# Take-home — Document Control Register

A small, self-contained .NET task. Time-box **~4–6 hours**. No external services, database or
credentials are needed — everything runs locally.

*(Zadání je níže i v češtině.)*

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
- Fill in the **"Candidate notes"** section at the bottom (English): key design decisions, the
  reading-tasks rule and why, trade-offs, and what you'd do with more time.
- Optional: a tiny CLI or minimal API to demo. Not required; tests are read first.

## What we look for

- A state machine that rejects illegal transitions.
- An audit trail that is genuinely append-only and **complete** — nothing slips through unaudited.
- The supersede invariant (one Published) and the reading-tasks judgment.
- Two-step destructive action + soft-delete discipline.
- Clean, readable C#; EF Core used sensibly; reasoning in your notes. We care as much about *why*
  as *what*.

---

## Zadání (CZ)

### Jednou větou
Napíšeš malou C# knihovnu, která spravuje **řízené dokumenty** (jako SOPy/směrnice) v jejich
životním cyklu a drží **úplnou, nezfalšovatelnou historii každé změny**. Žádné UI, žádné reálné
API — jen logika a testy, které dokážou, že se chová správně.

### Příklad — co tvoje knihovna dělá
Dokument (`SOP-001`) prochází stavy a **každá změna se zapíše do auditu**:

| Krok | Akce | Výsledný stav | Audit poté |
|---|---|---|---|
| 1 | Alice založí `SOP-001` v1 | **Draft** | `Created by alice` |
| 2 | Alice pošle k revizi | **InReview** | `… + Draft→InReview` |
| 3 | Bob schválí | **Approved** | `… + InReview→Approved` |
| 4 | Bob publikuje | **Published** | `… + Approved→Published` |
| 5 | Někdo zkusí publikovat `SOP-002` rovnou z Draftu | **odmítnuto** | *(nelegální přechod — beze změny)* |
| 6 | `SOP-001` v2 dojde do Published | v2 **Published**, v1 → **Superseded** | *(obojí v auditu; jen JEDNA Published)* |
| 7 | Alice „smaže" v1 | v1 **Archived** *(ne smazání řádku)* | `… + Archived` — a chtělo to **dvoustupňové potvrzení** |
| 8 | Dotaz na historii `SOP-001` | — | **celý seřazený seznam**, nic nechybí |

**Úkol:** naimplementovat register tak, aby se tohle chovalo přesně takhle — nelegální přechody
odmítnout, nic nezměnit bez auditu, nová verze superseduje starou, nic se nemaže natvrdo — a napsat
testy, které to dokážou.

### Pravidla
1. **Jen legální přechody** (`Draft→InReview→Approved→Published`, `Published→Superseded`,
   `→Archived`). Cokoliv jiného **odmítnout**.
2. **Neměnný, úplný audit** — každý přechod i editace metadat → záznam (kdo/kdy/z→do/důvod).
   Audit se nikdy neupravuje ani nemaže; **žádná cesta změnit dokument bez auditu**.
3. **Jen jedna Published** na číslo — nová verze superseduje starou.
4. **Jen soft-delete** — „smazání" = `Archived` s auditem. Žádný hard delete.
5. **Dvoustupňové potvrzení** u archivace (token / druhé volání — tvůj návrh).
6. **Úsudková otázka:** co s nevyřízenými reading tasky při supersede (přenést/uzavřít/
   znovuotevřít)? Vyber, naimplementuj, **zdůvodni** v poznámkách.

### Co odevzdat
- Tuto solution s **EF Core / SQLite**, logiku jako **testovatelné jádro** (`now`/`actor` se
  předávají). **Unit testy** pravidel a hran. Vyplň **„Candidate notes"** dole (anglicky).
- `dotnet test` musí projít z čistého klonu.

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

---

## Candidate notes (fill this in)

> Key design decisions, your reading-tasks rule and why, trade-offs, and what you'd do with more time.

_(your notes here)_
