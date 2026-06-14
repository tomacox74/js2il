# ADR 0007: test262 CI Suites and Reporting Workflow

- Date: 2026-04-20
- Status: Accepted

## Context

Issue #932 follows the intake/bootstrap work (#928), metadata parser (#929), MVP runner (#930), and classified `summary.json` baseline artifact (#931).

At this point JROC already has:

- a local manual runner entry point (`npm run test262:run-mvp`)
- a bounded MVP contract
- a machine-readable summary artifact for each invocation

What it still lacks is a repository-level CI/reporting story:

- PR validation needs a **small deterministic smoke slice** instead of an unbounded test262 run
- scheduled/manual GitHub runs need a **larger bounded slice**
- CI needs to **publish** the machine-readable summary instead of keeping it local-only
- scheduled regressions should open or update a tracking issue the same way the differential workflow already does

## Decision

JROC adds **named test262 MVP suites** plus a dedicated **`test262 MVP` GitHub Actions workflow**.

## Named suites

Suites are defined in `tests/test262/mvp-suites.json`.

- `pr`
  - small curated file list
  - intended for PR and push validation
- `nightly`
  - broader bounded `test/language/` slice with an explicit limit
  - intended for scheduled and manual workflow runs

The runner now accepts:

- `--suite <name>`
- `--suite-config <path>` (primarily for testing/overrides)

and records named-suite metadata in `summary.json` under `selection.namedSuite`.

## CI workflow

`.github/workflows/test262-mvp.yml` runs two bounded modes:

1. **PR suite**
   - on `pull_request`, `push`, and manual dispatch with `suite=pr`
2. **Nightly suite**
   - on `schedule` and manual dispatch with `suite=nightly`

Both jobs:

- restore dependencies
- build JROC in Release
- bootstrap the pinned test262 intake
- run the named suite
- upload `summary.json` as an artifact

On failure, the workflow also uploads the full test262 run directory for debugging.

## Failure tracking

Scheduled nightly failures reuse the repository's existing scheduled-workflow issue pattern:

- ensure a dedicated label exists
- comment on an existing open tracking issue when possible
- otherwise create a new issue with the workflow run URL

This keeps nightly failures visible without spamming duplicate issues.

## Consequences

### Positive

- Contributors and CI now share the same bounded suite definitions.
- PR validation stays bounded and review-friendly.
- Scheduled/manual runs publish machine-readable reports suitable for triage and backlog refreshes.
- The nightly failure signal is explicit and durable.

### Negative

- The named suites are intentionally heuristic rather than comprehensive coverage claims.
- The curated PR suite will need occasional review if the pinned upstream commit changes enough to make the chosen files less representative.

### Mitigations

- Keep suite definitions small and checked in for review.
- Record the named suite in `summary.json` so historical artifacts stay attributable.
- Keep the nightly slice bounded even as the runner grows.
