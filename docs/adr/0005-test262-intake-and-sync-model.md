# ADR 0005: test262 Intake and Sync Model

- Date: 2026-04-11
- Status: Accepted

## Context

Issue #927 introduces a phased test262 conformance program, and Issue #928 is the first dependency because every later runner/reporting decision depends on how JS2IL acquires and pins upstream `tc39/test262`.

We need one canonical intake model that keeps:

- contributor setup low-friction
- CI reproducible
- repository churn manageable
- licensing and attribution explicit
- Windows line-ending behavior safe for upstream tests

The candidate models were:

1. a sibling checkout outside the repository
2. a vendored snapshot committed into this repository
3. a scripted fetch flow pinned to an exact upstream commit

## Decision

JS2IL uses a **scripted fetch + pinned commit** model for test262 intake.

- The canonical pin lives in `tests/test262/test262.pin.json`.
- Local bootstrap and CI both use `node scripts/test262/bootstrap.js`.
- The managed checkout lives outside source control under `artifacts/test262/cache/<sha>`.
- Developers may override the managed checkout with `JS2IL_TEST262_ROOT` (or `--root`) for local experimentation, but that override is not the canonical path and must not become the required workflow.
- The managed checkout is materialized as a sparse git checkout with `core.autocrlf=false` and `core.eol=lf` so Windows does not rewrite upstream files to CRLF.

## MVP Intake Scope

The pinned intake keeps only the minimum upstream content needed for the first plain synchronous-script phase:

- root metadata files:
  - `LICENSE`
  - `INTERPRETING.md`
  - `features.txt`
  - `package.json`
- harness content:
  - `harness/**`
- first-wave test roots:
  - `test/language/**`
  - `test/built-ins/**`

The MVP intentionally excludes:

- `test/annexB/**`
- `test/intl402/**`
- `test/staging/**`
- module-flagged tests
- async-flagged tests
- agent/broadcast-heavy tests
- async-harness-dependent cases

The file-level exclusions inside `test/language/**` and `test/built-ins/**` are enforced later by the metadata parser and runner work (#929-#931); Issue #928 only defines the canonical upstream intake boundary.

## Local Flow

The default local workflow is:

1. run `npm run test262:bootstrap`
2. let the script materialize or reuse `artifacts/test262/cache/<sha>`
3. point later tooling at that resolved root (or call `npm run test262:root`)

For local-only experimentation, a developer may point `JS2IL_TEST262_ROOT` at a sibling checkout or another prepared test262 directory. The bootstrap script still validates the expected root files, harness files, and pinned `package.json` version before accepting the override.

## CI Flow

CI is allowed to fetch the pinned commit from GitHub and cache by SHA.

The expected pattern is:

1. restore `artifacts/test262/cache` with a cache key derived from `tests/test262/test262.pin.json`
2. run `npm run test262:bootstrap`
3. let downstream runner/reporting jobs consume the resolved root

This keeps the repository small while still making CI deterministic.

## Licensing and Attribution

JS2IL does not vendor the upstream suite into the repository for this phase, but the managed checkout still preserves upstream attribution material.

- The sparse checkout always includes `LICENSE` and `INTERPRETING.md`.
- Any later generated baselines, mirrored files, or exported reports that copy upstream content must preserve the relevant upstream notices.
- The pinned `package.json` version is recorded alongside the commit SHA so interpretation-shift changes in upstream test262 remain visible during pin bumps.

## Update Policy

Pin bumps are **manual**.

- Update `tests/test262/test262.pin.json` with a new commit SHA and matching upstream `package.json` version.
- Re-run `npm run test262:bootstrap`.
- Summarize the upstream commit, package version, and any material harness/interpretation changes in the PR description.

This keeps test262 updates explicit and reviewable instead of introducing silent scheduled drift before the runner/reporting pipeline exists.

## Consequences

### Positive

- Reproducible local and CI intake without committing a large third-party tree.
- Cache-by-SHA works naturally once CI is wired in.
- Later issues can build on a real pinned root path instead of inventing one ad hoc.
- Windows line endings are controlled in the managed checkout instead of depending on contributor git settings.

### Negative

- First bootstrap requires git network access.
- There is one more small piece of repo configuration (`test262.pin.json`) to maintain.
- Local override roots can drift from the canonical pin if used casually.

### Mitigations

- Keep the canonical path script-first and validate override roots before use.
- Keep pin bumps manual and reviewable until the later reporting workflow exists.
- Store the pin in-repo so runner and CI changes can key from one source of truth.

## Alternatives Considered

### 1) Vendored snapshot committed into JS2IL

Rejected because upstream test262 is large, would create noisy update PRs, and is unnecessary now that CI may fetch and cache by SHA.

### 2) Sibling checkout as the primary workflow

Rejected because it makes CI and contributor onboarding less reproducible and would force every later tool to guess where the suite lives.

### 3) Git submodule

Rejected because the repository does not currently use submodules, and a scripted pinned fetch gives the same reproducibility with less contributor friction.
