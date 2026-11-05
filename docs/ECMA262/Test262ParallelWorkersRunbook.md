# Operating the Test262 worker pool

The central catalogue workflow prepares one immutable allocation, runs execution-only
workers on separate Actions runners, then reconciles their receipts before native
sealing. The database advertises a maximum of four execution slots. Keep the repository
default at one until a representative production pilot supports changing it.

## Dispatch and observe

Use **Test262 central catalogue** on `master`. Select `mvp-composite` for a regression
collection or `native` for discovery, and an explicit coherent area for a bounded
pilot. `workers=configured` uses `TEST262_SPLIT_WORKER_COUNT`, falling back to one.
A manual dispatch can select one, two or four and enable split execution for that
dispatch without changing the weekly default.
Every selected slot needs its own existing producer variable and protected
`test262-catalogue-worker-N` environment credential. Preparation rejects a missing
producer before it registers the allocation. `TEST262_SPLIT_WORKFLOW=true` enables
the split path. The weekly Monday 06:15 UTC trigger selects native discovery.

The prepare and finalize job summaries show the exact source revision, provenance,
worker producer/run/job identities, pending/leased/completed/deferred/cancelled work,
expired leases and missing rows. The JSON snapshots are retained for 90 days. Pending
means enrolled work awaiting a claim. Expired leases remain a subset of leased work
until lifecycle settlement. Throughput is screening attempts per active minute from
stored metrics and remains unknown until those metrics exist.

While a collection is active, dispatch **Test262 collection status** on `master` with
the original central Actions run ID. It reads the frozen allocation and current worker
states through the authenticated catalogue API and emits the same structured summary.
Missing work, expired leases or a running worker whose Actions job has ended require
inspection of the original job and recovery artifacts. Snapshot reads can span concurrent
commits, so timestamps and counts are observational rather than a sealing receipt.

## Cancellation and partial failure

The execution matrix uses `fail-fast: false`. A failed worker leaves its peers within
the original shared attempt/time limits. Finalization requires every registered slot's
exact Actions job and retained recovery evidence. A failed or cancelled worker blocks
native sealing and publication until recovery is resolved.

Cancel the original Actions run to stop its jobs. Wait for all original execution jobs
to become terminal. Inspect **Test262 collection lifecycle reconciliation**, which runs
after the central workflow completes. If needed, manually dispatch that workflow with
the exact Actions run ID and attempt, first inspecting and then applying the reviewed
terminal reconciliation. Cancellation can interrupt uploads or finalization. A cancelled
job alone does not prove that its outbox was flushed or its budget settled.

Preserve `test262-split-prepare-RUN-1`, every `test262-split-worker-RUN-1-SLOT`, and
`test262-split-finalize-RUN-1`. The worker artifact contains its SQLite outbox snapshot.
Use the existing upload-only recovery path with the original producer, repository,
authority epoch and frozen context. Replaying uses the same request/observation UUIDs
and lease generation. Repeating a flush must retain its acknowledged receipts. An old
worker's missing artifact or revoked credential requires operator investigation before
acceptance. The #2365 acceptance record documents the tested fault and replay cases.

Do not rerun a split job through Actions' rerun control. Split execution requires attempt
one. After original jobs stop, replay pending receipts and reconcile leases/budgets using
their original ownership. Start a new dispatch for remaining eligible work. The database
retains completed observations, generation fences, cumulative accounting and discovery
cursor state. A new dispatch receives its own bounded budget. Review that additional
allocation before using repeated dispatches to recover a large backlog.

## Roll back to one worker

1. Pause manual dispatches and keep the next weekly trigger outside the maintenance
   window. Cancel an active collection if necessary and wait for its original jobs to stop.
2. Inspect/reconcile each original collection and resolve its outboxes and outstanding
   reservations. Confirm no active worker or live lease remains from that allocation.
3. Set `TEST262_SPLIT_WORKER_COUNT=1` for future scheduled/default dispatches. For a
   single rollback check, dispatch with `workers=1` while keeping the split workflow enabled.
4. Run the same bounded area and verify one execution job, its retained receipts, terminal
   collection, settled reservations and independent native validation when a patch exists.
   Compare evidence and registration counts using the run's recorded provenance.

Reducing the worker count affects new allocations. An existing four-slot plan retains
four slots. Keep the additive schema, original worker identities, outboxes and budgets
through recovery. Switching `TEST262_SPLIT_WORKFLOW` off selects the legacy supervisor
for future runs and requires separately verified compatibility after managed work is
quiescent. Returning to one split worker is the normal concurrency rollback.

## Budgets, concurrency and publication

Workers share one frozen candidate pool and aggregate attempt/time limits. The database
serializes claims and fences lease generations. Actions serializes central collections
of the same evidence kind and uses a separate shared target-preparation group for native
registration refresh. Increasing slots changes parallelism within that allocation.

Native evidence must pass collection reconciliation, current-source checks, sealing and
focused validation in a job without catalogue credentials. Publication additionally
requires an explicit manual native dispatch with `publish=true` and the protected
publication environment. Scheduled runs collect/validate evidence automatically.

## Rollout acceptance

Record the four-worker pilot and subsequent one-worker rollback run IDs, exact source and
provenance, work/evidence counts, worker job receipts, registration snapshot and validation
results in #2367. Use a coherent area with enough eligible variants to exercise the pool,
while retaining existing candidate/attempt/time limits. Interpret correctness by fixture
outcomes and lease receipts, and compare full job elapsed time and runner minutes before
raising the default. #2366 supplies a ten-variant infrastructure baseline.
