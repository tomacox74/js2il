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
4. Offer workflow_dispatch with revision, feature filter, candidate limit
   (default 200 distinct fixtures, maximum 500), accepted limit (default 100,
   maximum candidate limit), variant execution budget (default 400), total
   time budget (default 20 minutes), and dry-run. Limits apply to the entire
   run, not independently to each shard. Count fixture files, execution variants
   and accepted ports separately. Timeouts and bisection retries consume budgets.
5. Serialize native-porting writers with one workflow concurrency group, separate
   from the read-only MVP catalog importer. Do not rely on workflow concurrency
   as a durable FIFO. Persist the last reconciled successful master revision and
   rescan the unprocessed ancestor range at each wake-up. A scheduled recovery
   run must reconcile missed events. Coalesce changes into the latest validated
   descendant, recording the full covered range; never claim attribution to one
   fix when the retry spans several merges.
6. Avoid recursive runs: registration/documentation-only batch merges do not
   qualify. Never auto-merge generated PRs.

## Durable state

Add a separate native-screening evidence store rather than mutating MVP result
meaning. Record fixture full path and upstream hash, pin, all required variants,
compiler/runtime binary identity, native harness identity, execution environment,
phase, diagnostic signature, outcome and timestamps.

Record automation run ID, trigger/base revisions, selected paths, budgets,
native acceptance artifacts, generated branch/PR, deferred groups and completion
state. Track attempted outcomes, not only successes. Use versioned SQLite tables
in an automation-owned database, importing MVP exports read-only. Include
pending work items with cursors and last-attempt build identity; advancing the
reconciled-merge cursor must not discard candidates deferred by a budget.
Commit local outcomes transactionally. Only advance the durable cursor after
its checkpoint upload succeeds. On restart, reconcile any published branch/PR
before retrying publication; a crash between publication and checkpointing must
not create a second PR.

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

Allocate 80% of the candidate budget to relevant historical failures and 20%
to a rotating fallback/discovery slice; redistribute unused capacity. Import MVP
failure diagnostics as labeled hints and seed native failures from preserved
screening artifacts when available. Missing native history is a normal cold
start, not a reason to stop. Within the selected coherent area, allow cached
passing candidates to fill spare capacity after the retry allocation. Exclude support files and explicitly unsupported requirements with
machine-readable reasons. Preserve current registration exclusions and compare
full paths rather than basenames.

## Execution stages

### 1. Restore and plan

Restore trusted default-branch catalog and native-screening checkpoints. Refresh
registration inventory against the checked-out target revision. Verify the pin
and inventory completeness. Emit a deterministic plan and selection reasons.
If state is missing, report a cold start and use bounded discovery.

### 2. Build once and screen

Reuse the native harness execution APIs rather than maintaining a second
implementation. Provide a screening host or isolated temporary generated test
project; never require permanent fixture registrations just to screen. An
unsupported metadata/helper/phase combination is a harness gap, not a pass.

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
Record the target SHA and run acceptance for the generated branch. If master
advances, mark evidence as awaiting refresh; refresh in a bounded maintenance
run or through required integration CI. Do not rebase indefinitely on every
push. Never overwrite human edits: verify the expected bot branch head before
updating it, and pause bot updates when ownership is no longer exclusive.

### 5. Open or update one draft PR

For version one, allow only one open bot porting PR repository-wide. Continue
checkpointing discovery while it is open, but defer publication of another
batch. This removes the need for a distributed candidate-reservation service.
Use an isolated bot branch and a stable run/batch identity in PR metadata.
Reconcile live open and closed PRs and native registrations on restart. Do not
silently recreate a closed unmerged batch; defer its candidates with the closure
reason until explicitly retried. Multi-PR reservations are a later optimization.

Include trigger/build/pin provenance, candidates screened, accepted count,
deferred outcomes, affected features, exact validation commands/results and
artifact links. Update Index.md, Test262Conformance.md and CHANGELOG.md as
required by porting instructions.

Open as draft until required CI is green and evidence is complete. A human
reviews and merges. Empty batches publish a summary, not an empty PR.

## Deterministic generation and permission boundary

Version one does not require a coding agent or model subscription. Implement
selection, faithful fixture copying, registration generation, coverage calculation
and PR publication as deterministic commands. Agents may later investigate
reported semantic failures through separately authorized tasks. Product fixes
remain outside this coverage-only pipeline.

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

1. Implement a manually dispatched vertical slice for one existing feature
   folder: import catalog hints, screen at most 20 fixtures with the existing
   native harness, generate unchanged fixtures/registrations, recompute coverage,
   and upload a validated patch and report. No publication credential required.
2. Add versioned native evidence, transactional checkpoints, budgeted resume,
   group bisection and diagnostic classification.
3. Configure a GitHub App publication identity with repository-scoped contents
   and pull-request permissions. Validate one generated draft PR and its normal
   CI end-to-end; this requires credential configuration, not an agent adapter.
4. Add successful-master-validation triggers, cursor-based reconciliation,
   a scheduled recovery wake-up and conservative component mapping.
5. Extend supported feature folders and metadata/dependency shapes using
   capability checks. Keep unsupported shapes deferred until supported.
6. Add multi-PR throughput or optional issue reporting only if measured demand
   warrants their additional complexity.

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
- simultaneous/restarted runs cannot publish duplicate candidates or PRs;
- a dropped trigger is recovered from the durable merge cursor;
- budgets leave resumable work rather than silently advancing past it;
- bot updates preserve human edits and closed unmerged batches stay deferred;
- candidate, accepted and variant limits hold across shards and bisection;
- master advancement invalidates acceptance until rerun;
- expired artifacts produce explicit cold-start behavior;
- empty or failing batches do not publish misleading PRs;
- publication credentials are unavailable to fixture execution.

## First-release acceptance criteria

A manually dispatched run for one supported folder must produce a patch and
machine-readable report without a model call or hand-edited registration.
Require exact upstream bytes, complete required-variant evidence, focused native
acceptance, deterministic registrations, and regenerated coverage rows whose
counts reconcile with the pinned corpus. Registration inventory alone is not
proof of execution: correlate accepted fixture paths with native test results.
Preserve all existing coverage and document development-versus-release attribution.

Dry-run may write scratch files and upload reports; it must not push branches,
open PRs/issues or mutate the production state cursor. Validate generated patches
against an explicit allowlist: fixture/dependency paths, registration files and
coverage/changelog files only. Reject workflow, production code and unrelated
documentation edits. Publication must verify the patch digest and trusted run
identity and must not execute generated code in its credential-bearing job.

A failure-only run is successful when diagnostics and resumable work are
checkpointed; it creates no PR. A partial acceptance batch may publish only its
fully verified subset, with other outcomes retained explicitly.

## Success metrics

Report elapsed time and accepted tests per hour, split into restore/planning,
build, screening, generation, native acceptance and publication. Also track
cache reuse, repeated attempts, failures unlocked by each merge, duplicates
prevented, timeouts and PR review/rework rate.

Success means less human orchestration and more trustworthy accepted tests per
reviewable PR, not simply more copied fixtures.
