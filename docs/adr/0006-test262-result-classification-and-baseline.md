# ADR 0006: test262 Result Classification and Baseline Artifact

- Date: 2026-04-13
- Status: Accepted

## Context

Issue #931 follows the pinned intake/bootstrap work (#928), metadata parser (#929), and first MVP runner (#930).

The initial runner can execute the bounded plain-script slice, but it only emits ad hoc `PASS` / `FAIL` / `SKIP` text. That is not enough for conformance triage because JS2IL still needs to distinguish:

- expected parse-phase rejection vs runtime-phase rejection
- wrong-phase failures
- wrong runtime error kind
- policy skips vs unsupported requirements
- a stable artifact that later CI/reporting work can publish or compare

We also want to keep the policy source of truth small and reviewable instead of introducing another large checked-in manifest too early.

## Decision

JS2IL adopts a **result-kind + verdict** model for the test262 MVP runner and uses the runner's `summary.json` as the baseline artifact.

### Result model

Each classified entry records:

- a stable `kind`
- a `verdict`
- the observed phase (`selection`, `compile`, or `runtime`)
- machine-readable reasons for not-run entries
- a stable repro descriptor (`file` + optional `variant`)

The current MVP kinds are:

- `pass`
- `compile-rejection`
- `runtime-rejection`
- `wrong-phase`
- `wrong-error-kind`
- `runtime-mismatch`
- `timeout`
- `runner-error`
- `unsupported-requirement`
- `skipped-by-policy`

The current verdicts are:

- `matched`
- `unexpected`
- `not-run`

### Policy sources

There is **not** a second dedicated checked-in policy file for the MVP.

Instead, the machine-readable policy is split across the existing sources that already own that information:

1. `tests/test262/test262.pin.json#excludedFromMvp`
   - path-based policy exclusions only
   - this field no longer carries pseudo-frontmatter rules
2. `scripts/test262/metadataParser.js`
   - `unsupported` codes describe metadata the runner cannot model yet
   - `mvpBlockers` codes describe harness/runtime requirements outside the MVP slice
3. upstream test262 frontmatter
   - `negative.phase` and `negative.type` remain the canonical expectation source for negative tests

### Negative-test expectations

- `negative.phase=parse` is currently validated at the **compile** phase only. JS2IL does not yet surface normalized parser error objects, so parse negatives are phase-classified but do not yet assert parse error kind.
- `negative.phase=runtime` is validated at the **runtime** phase and does check the observed runtime error kind.

### Baseline artifact

Every non-list runner invocation writes:

- `<output>/summary.json`

That JSON file is the baseline/report artifact for the MVP. It includes:

- pin identity (`commit`, `packageVersion`)
- the effective path-exclusion policy
- the negative-phase expectation contract
- selection counts
- verdict/kind counts
- per-entry classification records

To keep the artifact stable across operating systems, runtime observations normalize process exits to pass (`0`) vs rejected (`1`) instead of preserving OS-specific non-zero values for thrown failures.

This is the artifact later CI work (#932) should publish or compare. For checked-in baselines, contributors should use a bounded deterministic selection (for example `--file` or a narrow `--filter`) and commit only `summary.json`, not the generated compiled outputs.

## Consequences

### Positive

- Conformance output becomes actionable without pretending the MVP slice is a full-suite result.
- Policy skips and unsupported requirements are distinguished instead of both collapsing into generic `SKIP`.
- The baseline artifact is stable enough for later CI/reporting work without introducing a second large manifest prematurely.

### Negative

- Parse-negative kind checking is still incomplete because compiler diagnostics are not yet normalized to JS error objects.
- Path exclusions still live in the pin file even though later phases may eventually want a richer dedicated policy surface.

### Mitigations

- Record the parse-negative limitation explicitly in `summary.json` and this ADR.
- Keep the result schema narrow and versioned so a future dedicated policy or richer baseline compare step can evolve without breaking the MVP contract silently.
