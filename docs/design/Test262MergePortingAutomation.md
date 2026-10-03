# Merge-triggered Test262 porting automation

Status: design proposal; no unattended execution is enabled by this change.

## Goal

After a compiler/runtime/harness change lands on master, retry a bounded set of
previously failing, unregistered Test262 fixtures and prepare a reviewable PR
containing newly passing native tests. Eliminate repeated manual candidate
discovery while preserving upstream fidelity and independent native acceptance.

## Existing infrastructure

Reuse scripts/test262/catalog.py and .github/workflows/test262-catalog.yml.
The existing SQLite catalog records MVP composite-runner variants, binary and
environment provenance, historical outcomes, registration inventory and
deterministic exports. It is not a native acceptance database. Do not reinterpret
MVP passes or parse-negative rejection as native conformance.

The existing catalog workflow serializes writers by branch, restores its own
trusted artifacts and aggregates four independent shards. Preserve those
invariants rather than adding another concurrent writer to the same database.

Follow .github/skills/test262-porting/SKILL.md and
.github/skills/test262-batch-porting/SKILL.md for all generated ports.

## Trigger and ownership

1. Trigger after the master validation workflow succeeds for a master push.
   Resolve the exact triggering commit; do not substitute whatever master points
   to when the worker eventually starts.
2. Derive changed files against the previous successfully processed master
   commit. A first run, force push or unavailable ancestor requires a bounded
   manual baseline, not a full-corpus scan.
3. Process compiler, runtime, native harness, pinned corpus and catalog-tooling
   changes. Documentation-only changes should not launch screening.
4. Offer workflow_dispatch with revision, feature filter, candidate ceiling
   (default 100, maximum 500), time budget and dry-run.
5. Serialize automation writers repository-wide. Retain queued commit ranges
   rather than silently discarding changes when a newer run arrives.
6. Avoid recursive runs: registration/documentation-only batch merges do not
   qualify. Never auto-merge generated PRs.

## Durable state

Add a separate native-screening evidence store rather than mutating MVP result
meaning. Record fixture full path and upstream hash, pin, all required variants,
compiler/runtime binary identity, native harness identity, execution environment,
phase, diagnostic signature, outcome and timestamps.

Record automation run ID, trigger/base revisions, selected paths, budgets,
native acceptance artifacts, generated branch/PR, deferred groups and completion
state. Track attempted outcomes, not only successes.

Historical outcomes are selection hints. Every accepted fixture requires fresh
native acceptance on the exact target build. Preserve failure and incomplete
outcomes when a worker times out.

Initially use trusted Actions artifacts with explicit retention and an expiration
warning, matching existing catalog conventions. A central database can replace
this later; it is not a prerequisite and must not be assumed configured.

## Failure selection and invalidation

Maintain a reviewed component-to-feature map and use diagnostic signatures to
associate failures with likely components. Examples:

- callable lowering: async parameter initialization, generator invocation,
  class method planning;
- private-member lowering: brands, nested closures, static/instance access;
- property operations: computed keys, coercion, descriptors and Proxy/Reflect;
- iterator/Promise runtime: capabilities, abrupt completion and closing;
- regex runtime: exec dispatch, flags, species and Symbol operations.

This map is a prioritization heuristic, not proof of dependency. Include a
bounded rotating fallback slice so unmapped failures cannot starve. Broad
shared-operation changes increase the affected set but never remove budgets.

Select unregistered failures first, then related historical passes if capacity
remains. Exclude support files and explicitly unsupported requirements with
machine-readable reasons. Preserve current registration exclusions and compare
full paths rather than basenames.

## Execution stages

### 1. Restore and plan

Restore trusted default-branch catalog and native-screening checkpoints. Refresh
registration inventory against the checked-out target revision. Verify the pin
and inventory completeness. Emit a deterministic plan and selection reasons.
If state is missing, report a cold start and use bounded discovery.

### 2. Build once and screen

Build the current compiler/runtime once. Respect required strict/non-strict
variants and native helpers. Use bounded native screening for candidates that
the MVP runner excludes, such as supported async or agent fixtures; do not drop
them simply because the MVP intake cannot execute them.

Screen temporary groups using the native harness. If group compilation fails,
bisect in temporary screening only. Keep committed fail-whole-folder behavior.
Enforce per-test, per-group and total timeouts; clean up workers and checkpoint
partial results.

### 3. Prepare a coherent batch

Take up to 100 freshly passing candidates by default, within one coherent area.
Preserve upstream bytes, copyright, metadata and sibling dependencies. Generate
registrations through existing conventions, check identifier/display-name
collisions, and reject duplicate legacy registrations.

Do not modify product code, weaken assertions or turn failures into skips in
this coverage-only automation. Report semantic fixes as separate work.

### 4. Native acceptance and evidence

Validate copied bytes against the pinned source. Run focused native tests for
the affected namespaces, build affected projects, and refresh coverage totals
from unique standalone applicable upstream files.

A native pass requires the declared outcome, phase and error type. Never use
MVP evidence as the acceptance gate. Full CI remains required on the PR.
If the target branch advances, rebase and repeat acceptance before declaring
the batch ready.

### 5. Open or update one draft PR

Use an isolated bot branch. Reconcile existing automation PRs before creating
another; reserve candidate paths through persisted ownership records.
Revalidate reservations against live registrations and branches on restart.

Include trigger/build/pin provenance, candidates screened, accepted count,
deferred outcomes, affected features, exact validation commands/results and
artifact links. Update Index.md, Test262Conformance.md and CHANGELOG.md as
required by porting instructions.

Open as draft until required CI is green and evidence is complete. A human
reviews and merges. Empty batches publish a summary, not an empty PR.

## Agent integration and permission boundary

Choose an explicit supported coding-agent adapter before implementing PR
generation. Do not assume that a GitHub Actions token can invoke Copilot or
that a CLI subscription/token is configured.

Separate read-only screening from the publication job. The latter needs only
contents:write and pull-requests:write, with a dedicated authorized bot identity
if generated pushes must trigger normal CI (GITHUB_TOKEN-originated events may
not trigger follow-on workflows). Verify the selected integration's behavior.

Do not use pull_request_target to run untrusted code, restore PR artifacts as
trusted state, evaluate fixture content as shell instructions, or expose
publication credentials to screened JavaScript. Agent-generated changes are
untrusted until validation. Require protected-branch controls and never write
directly to master.

## Deduplicated failure reports

Initially publish failure clusters in artifacts and the run summary. Automated
issue creation is opt-in and separate: stable root-cause signatures, existing
issue lookup, representative fixture paths, exact diagnostics and affected
counts. Do not create one issue per fixture or infer identical causes solely
from matching exception names.

## Implementation sequence

1. Add native-screening evidence schema, deterministic planning CLI and tests.
2. Add bounded native screening and temporary-group bisection with checkpointing.
3. Add faithful batch generation and machine-derived coverage updates.
4. Add manually dispatched dry-run workflow.
5. Configure agent/publication identity and validate one draft PR end-to-end.
6. Enable successful-master-merge triggers and conservative component mapping.
7. Add optional deduplicated issue reporting after observing cluster quality.

Each stage should be independently reviewable. Do not turn on merge-triggered
publication before manual end-to-end acceptance succeeds.

## Required tests

- strict/non-strict results cannot combine across provenance;
- stale binary/harness/pin evidence cannot qualify a native acceptance;
- runtime and parse negatives require their exact expected outcome;
- registered and legacy duplicate paths are excluded;
- unsupported and support files never become passes;
- interrupted screening resumes without losing completed results;
- bisect identifies a poisoned fixture without changing suite semantics;
- byte changes, missing siblings and identifier collisions block publication;
- unrelated changes produce no screening run;
- unmapped failures receive bounded fallback selection;
- simultaneous/restarted runs cannot reserve or publish duplicate candidates;
- master advancement invalidates acceptance until rerun;
- expired artifacts produce explicit cold-start behavior;
- empty or failing batches do not publish misleading PRs;
- publication credentials are unavailable to fixture execution.

## Success metrics

Report elapsed time and accepted tests per hour, split into restore/planning,
build, screening, generation, native acceptance and publication. Also track
cache reuse, repeated attempts, failures unlocked by each merge, duplicates
prevented, timeouts and PR review/rework rate.

Success means less human orchestration and more trustworthy accepted tests per
reviewable PR, not simply more copied fixtures.
