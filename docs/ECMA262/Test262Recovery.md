# Test262 retention and recovery acceptance (#2343)

**Reviewable procedures and proposals, not approval or deployment.** No production
restore/cutover, credential rotation, storage/KMS policy or authority change is
authorized by this document. Changes to scheduled tooling take effect only after
review/merge; no live drill was dispatched for this work.

## Proven scope and remaining decisions

[Backup 39882296770](https://github.com/tomacox74/js2il/actions/runs/39882296770)
and [restore 39883218310](https://github.com/tomacox74/js2il/actions/runs/39883218310)
prove one independently retained, exact-version encrypted backup and disposable
PostgreSQL 17 recovery: 267,160 fixtures, 213,728 annotations, shadow authority,
disabled worker bindings, `production_cutover=false`, finite COMPLIANCE retention
through November 27, 2026. They do not prove current backup freshness, indefinite
retention, complete archives, measured service recovery objectives or production rollback.
#2342 separately accepted its coherent snapshot; **11 unavailable historical gaps**
remained at that snapshot. #2242 retains the original gap ledger. Never infer
`history_complete=true` from this document, a green workflow or a coverage report.

| Owner decision required | Concrete recommendation | Evidence needed before acceptance |
| --- | --- | --- |
| Recovery point objective | **Proposed:** at most 24h data loss | Adopt objective; measure snapshot age and any acknowledged post-snapshot replay boundary |
| Recovery time objective | **Proposed:** at most 4h full service recovery | Adopt objective; measure approval/access/provisioning/restore/replay/fencing/verification/resume, not just database restore |
| Compact evidence | **Proposed:** indefinite sanitized receipts, digests, provenance and gap decisions | Approve ownership/cost/access and independently retrievable retention policy |
| Verbose logs | **Proposed:** 180-day hot retention, then useful hashed archival | Define useful/restricted contents, filtering/redaction, archival ownership, retrieval and deletion approval |

Existing tooling's minimum **35-day COMPLIANCE default** and Actions' **90-day**
artifact convenience are implemented finite checks, not approval of those proposals.
Do not change bucket/KMS policies or retention settings as a consequence of this PR.

## Coverage, ownership and verification inventory

Owners below are operational roles and issue boundaries; the repository owner must
assign the accountable operator and backup before adopting the proposals.

| Evidence class | Existing mechanism / verified scope | Owner and cadence / missing gate |
| --- | --- | --- |
| Database dump and count/contract manifest | Daily 03:40 UTC `test262-central-backup.yml`; catalogue-only snapshot, no performance data | Backup operator, #2343; verify each exact immutable version/KMS/lock/readback and newest receipt; #2341 monitors backup age |
| Backup receipt | Published off-project only after dump and manifest readbacks; private retained receipt preserves storage identities | Backup operator, #2343; daily retrieval and retention checks; finite retention must not be mistaken for indefinite approval |
| Isolated restore report | Sunday 05:10 UTC `test262-retained-restore.yml`; master-only protected environment, no production DSN | Recovery operator, #2343; proposed code addition retains sanitized success/failure reports under existing finite Object Lock policy; verify first post-merge receipt without repeating old milestones |
| Original history archives and import/recovery reports | Reviewed `cutover-evidence.json` and existing preserving workflow cover selected eight artifacts/final five recoveries only | Archive operator, #2242; reconcile original ledger and exact versions/hashes; keep unavailable/expired/failed/unpreserved distinct |
| Failed/partial-run outboxes and useful supervisor logs | Existing central recovery artifacts and acknowledged outboxes; not a complete independently retained archive | Archive operator, #2242/#2343; preserve before Actions expiry, check SQLite/WAL integrity, hashes and acknowledgements; inventory absent/unrecoverable originals rather than rerunning fixtures/imports |
| Verification, reconciliation and drill compact records | Git/issue acceptance records, backup receipts, retained restore reports; sanitized #2342 record in #2397 | Evidence owner, #2343; per acceptance/drill and retention review; Git is a selected compact mirror, not raw archive proof |
| Operational samples and CI/verbose logs | Existing hourly #2341 monitor; 90-day Actions reports; release/manual performance cadence | Monitoring owner, #2341; proposed 180-day/archive policy not deployed; first successful sample and notification policy remain separate gates |

The metadata-only `recovery_coverage` command reuses the existing bounded workflow
receipt reader and reviewed archive manifest. The existing hourly monitor runs it
without database/object-store credentials; it introduces no second scheduler,
backup system, alerts or notification policy:

```sh
python -m scripts.test262.central.recovery_coverage --output /private/recovery-coverage.json
```

It reports latest failed/incomplete runs separately from older successful receipts,
unavailable/expired/invalid receipt proof, stale verification and recorded expired
object locks. It reads original-archive **metadata only**, distinguishes
unavailable/expired/mismatched metadata, and never substitutes metadata availability
for byte/digest/readback or independent storage proof. The existing reviewed set is
not an all-archive/all-log inventory. No fallback to older green evidence clears a
newer failure. Unknown proof remains unknown. Persist useful sanitized snapshots in
the existing issue/Git ledger; protect raw diagnostics and storage locations.

The **36h backup-age monitor threshold** belongs to #2341; it is not the proposed
24h RPO. An eight-day drill review window detects a missed weekly verification
without claiming a recovery guarantee. Coverage reports do not access S3 or prove
that a historical lock/readback is still valid. The owning operator must check
exact retained versions, retrieval/decryption and deadlines, including reports,
before expiry. Missing first receipts or unarchived logs remain acceptance gaps.

## Recovery measurements and isolated rehearsal

New backup manifests record `snapshot_started_at` from `transaction_timestamp()` in
the **same exported-snapshot transaction**, a conservative lower bound on recovery
point, not dump completion time. Legacy manifests remain usable but have
**unknown** recovery age; never derive it from `checked_at`. A future/naive recovery
timestamp fails explicitly instead of manufacturing a fresh backup.

The existing isolated verifier records `backup_age_seconds_at_start`,
`isolated_restore_seconds` and monotonic `elapsed_seconds`. Isolated time includes
restore and authority checks; total verifier time includes download/verification and
cleanup, excludes report retention and service provisioning/replay/resumption.
Neither is end-to-end RTO. Record any approved rehearsal's separate service timeline
and data-loss boundary; no objective is marked met merely because a tool finishes.

`--retain-report` uses existing exact-version KMS/Object Lock/readback helpers at
the existing destination's `restore-verification/<run>-<attempt>/restore-report.json`.
Both successful and failed drill reports are eligible once destination verification
succeeds. Archive/readback failure fails the job explicitly, without erasing the
restore outcome. If storage/access fails before a destination can be verified,
only the sanitized Actions failure report is available: investigate before expiry.
The report's public proof omits bucket, prefix, object keys, KMS identifiers and
unknown fields; the original private backup receipt remains usable by the selector.
This uses the existing finite policy, not an indefinite-retention implementation.
The operator must confirm that the existing role permits the approved report prefix;
an access denial is a failed acceptance gate, not authorization to broaden permissions.

Before an **explicitly approved new** isolated rehearsal:

1. Record accountable recovery operator, independent verifier, go/no-go owner,
   objective decisions, source version/epoch and incident identifier privately.
   Confirm retained version access/decryption, PG17/client versions, matching
   reviewed schema/API and required operator access. No production credentials,
   worker execution or permission broadening belongs in the disposable job.
2. Select a sufficiently current verified retained backup and exact versions.
   Keep the newest broken backup visible; do not silently fall back. Record
   snapshot lower bound, age, byte hashes, lock deadlines and unknown measurements.
3. Use the existing loopback-only, empty `catalogue_retained_restore` target.
   Validate exact counts, constraints, ACL/RLS/reporting behavior, provenance and
   absence of performance data. Restored bindings stay disabled and authority
   shadow at the incremented epoch, fenced in the restore transaction.
4. Exercise source/target loss, malformed/partial backup, failed retention/readback,
   post-snapshot acknowledged messages and stale-worker scenarios using disposable
   synthetic state only. Define expected rejection/no-dual-authority assertions
   before execution; do not copy live credentials or replay into production.
5. Preserve sanitized scope/timeline/outcome/limitations and exact-version proof.
   Check external report retrieval and record actual service-recovery phases;
   keep unexercised cutover/rollback gates open. Destroy only the approved
   disposable target after evidence collection.

## Operator-approved cutover and rollback decision procedure

This is a **go/no-go checklist to rehearse**, not an executed or verified production
path. The go/no-go owner must approve both direction and replay/data-loss boundary.
Do not publish a cutover command or replace a DSN as a shortcut around these gates.

1. **Quiesce:** stop new claims, producers, publication and ingestion on every
   writer path; account for in-flight requests, leases, reservations and durable
   outbox/completion acknowledgements. Record the last committed acceptance/replay
   boundary and approved retained snapshot privately. Pause/resume control is not
   a substitute for fencing independently authenticated writers.
2. **Fence first:** use the approved authority transition and credential plan to
   deny old and in-flight writer acceptance before enabling a target. A network
   partition or inability to prove the source fenced is **no-go**, not permission
   to activate the restored copy. Record a new monotonic epoch greater than any
   accepted source/target epoch; never reuse restored enabled bindings/credentials.
3. **Prepare shadow target:** verify source/schema/provenance and backup/replay
   parity, constraints/reporting/ACL/RLS and performance-data separation. Replay
   only the approved post-snapshot durable messages with original idempotency and
   verified receipts; distinguish pending, accepted and completion acknowledgements.
   Unknown acknowledgements/reservations require reconciliation, not duplicate
   fixture runs, invented observations, queue repair or automatic publication.
4. **Go/no-go:** the independent verifier records writer denial on the old side,
   target shadow/epoch checks, replay boundary and allowed data loss. Only an
   explicitly authorized owner can activate one target and bind new writers.
   Keep old authority fenced; verify rejected stale workers and acknowledged
   acceptance on the single chosen target before resuming the approved workloads.
5. **Partial failure:** before activation, keep target shadow and both sides
   non-accepting until the operator selects one provable authority. After target
   acceptance begins, freeze/fence that target and capture its new acknowledged
   boundary before any reversal. A failed report, routing switch or client timeout
   is not proof that activation/acceptance did not occur. Prefer safe downtime
   over simultaneous writable authorities.
6. **Rollback:** decide whether forward repair or a prepared recovery target is
   safer. Reconcile target-era acknowledged writes and schema compatibility;
   replay only verified required deltas. Rollback is a **new** approved fenced
   transition at another higher epoch, never re-enabling the old dump's workers
   or old active epoch. Repeat parity, stale-worker rejection, single-authority
   and reporting checks before controlled resume. If the accepted boundary cannot
   be established, remain fenced and escalate the explicit data-loss decision.

## Remaining acceptance gates

#2343 stays open for owner policy/role assignments, #2242's independently verified
archive/gap coverage, first post-review retained measurement/report verification,
actual backup/log coverage and retrieval, approved/measured RPO/RTO, and an
operator-approved synthetic cutover/rollback rehearsal. #2341 owns alert deployment
and monitoring acceptance. Existing 11 conflicted/refresh batches and one publication
warning from #2342 remain owned by #2332/#2344, not repaired by this work.

### Retained point-in-time coverage observation

The [sanitized October 25 coverage record](evidence/recovery-coverage-20261025.json)
retains the metadata-only observation outside Actions expiry, including the
collector/manifest digests and source workflow revisions. Existing scheduled backup
40159759562 and isolated restore 40165197393 had successful matching receipt ZIPs;
their recorded object-lock deadline was November 29, 2026. No new workflow was
dispatched, backup created or restore executed to collect it.

Seven of the eight selected original artifacts had matching available metadata.
Artifact **11522307991** returned unavailable metadata; its independently retained
copy was **not assessed**. This is a retrieval/verification handoff to #2242, not a
new claim of irretrievable history, a change to its 11-gap ledger, or evidence that
the previously retained copy disappeared. All independent archive availability,
current S3 readback and broader log coverage remain outside this observation.
