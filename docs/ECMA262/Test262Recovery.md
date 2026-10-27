# Test262 retention and recovery acceptance (#2343)

**Broader procedures and proposals are not approval or deployment.** Only the
fixed 42-version retention extension below has separate scoped owner authorization. No production
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
#2342 separately accepted its coherent snapshot. On October 26 the owner accepted
the [41-container preservation ledger](evidence/historical-retention-ledger-2242.json)
and its **11 original historical gaps**, closed #2242 and reopened #2343 for ongoing
retention/verification. All 41 containers have dated exact-version proofs; that is
not an all-history/all-log completeness claim. Never infer
`history_complete=true` from this document, a green workflow or a coverage report.

| Owner decision required | Concrete recommendation | Evidence needed before acceptance |
| --- | --- | --- |
| Recovery point objective | **Proposed:** at most 24h data loss | Adopt objective; measure snapshot age and any acknowledged post-snapshot replay boundary |
| Recovery time objective | **Proposed:** at most 4h full service recovery | Adopt objective; measure approval/access/provisioning/restore/replay/fencing/verification/resume, not just database restore |
| Compact evidence | **Proposed:** indefinite sanitized receipts, digests, provenance and gap decisions | Approve ownership/cost/access and independently retrievable retention policy |
| Verbose logs | **Proposed:** 180-day hot retention, then useful hashed archival | Define useful/restricted contents, filtering/redaction, archival ownership, retrieval and deletion approval |

Existing tooling's minimum **35-day COMPLIANCE default** and Actions' **90-day**
artifact convenience are implemented finite checks, not approval of those proposals.
Do not change bucket/KMS policies or broader retention settings as a consequence
of these proposals. The separately authorized fixed-version extension below is
the only approved exception.

## Coverage, ownership and verification inventory

Owners below are operational roles and issue boundaries; the repository owner must
assign the accountable operator and backup before adopting the proposals.

| Evidence class | Existing mechanism / verified scope | Owner and cadence / missing gate |
| --- | --- | --- |
| Database dump and count/contract manifest | Daily 03:40 UTC `test262-central-backup.yml`; catalogue-only snapshot, no performance data | Backup operator, #2343; verify each exact immutable version/KMS/lock/readback and newest receipt; #2341 monitors backup age |
| Backup receipt | Published off-project only after dump and manifest readbacks; private retained receipt preserves storage identities | Backup operator, #2343; daily retrieval and retention checks; finite retention must not be mistaken for indefinite approval |
| Isolated restore report | Sunday 05:10 UTC `test262-retained-restore.yml`; master-only protected environment, no production DSN; merged tooling retains sanitized success/failure reports under existing finite Object Lock | Recovery operator, #2343; verify the first scheduled receipt from that implementation, including its self-version and measurements, without repeating old milestones |
| Original history archives and import/recovery reports | Owner-accepted 41-container ledger: eight earlier proofs plus 33 newly preserved original recovery ZIPs, 4,348,851,849 bytes total | Archive operator, #2343; exact-version metadata and retrieval checks, finite-lock review and approved longer-term protection; #2242's accepted scope is closed |
| Failed/partial-run outboxes and useful supervisor logs | The accepted frozen inventory covers its required recovery containers, including incomplete original reports; it is not an all-log inventory | Archive operator, #2343 with #2341; preserve useful new failures before expiry under an approved policy; do not reopen original gaps or rerun fixtures/imports to manufacture missing history |
| Verification, reconciliation and drill compact records | Git/issue acceptance records, backup receipts, retained restore reports; sanitized #2342 record in #2397 | Evidence owner, #2343; per acceptance/drill and retention review; Git is a selected compact mirror, not raw archive proof |
| Operational samples and CI/verbose logs | Existing hourly #2341 monitor; 90-day Actions reports; release/manual performance cadence | Monitoring owner, #2341; proposed 180-day/archive policy not deployed; first successful sample and notification policy remain separate gates |

The metadata-only `recovery_coverage` command reuses the existing bounded workflow
receipt reader and digest-pinned accepted 41-container ledger. The existing hourly monitor runs it
without database/object-store credentials; it introduces no second scheduler,
backup system, alerts or notification policy:

```sh
python -m scripts.test262.central.recovery_coverage --output /private/recovery-coverage.json
```

It reports latest failed/incomplete runs separately from older successful receipts,
unavailable/expired/invalid receipt proof, stale verification and recorded expired
object locks. **Schema 3** preserves the original 41 archive fields, earliest
historical deadline and `historical_findings`, then authenticates the separately
retained April extension below. `latest_verified_*` fields and current `findings`
use that matching dated proof without rewriting the accepted ledger. The newer
receipt is a separate verified target, not a forty-second archive. Missing,
invalid or future-dated proof produces an explicit unknown finding and does not
silently become healthy. At/after the recorded April deadline, expiry findings
return; expiry itself does not prove deletion. It does not re-inventory Actions
sources or assert a fresh S3 observation; historical acceptance and latest
recorded protection are separate fields. Ongoing findings
belong to #2343, not closed #2242. This is not an all-archive/all-log inventory.
No fallback to older green evidence clears a newer failure. Unknown proof remains
unknown. Persist useful sanitized snapshots in
the existing issue/Git ledger; protect raw diagnostics and storage locations.

The adopted **36h backup-age monitor threshold** belongs to #2341; it is not the proposed
24h RPO. An eight-day drill review window detects a missed weekly verification
without claiming a recovery guarantee. Coverage reports do not access S3 or prove
that a historical lock/readback is still valid. The owning operator must check
exact retained versions, retrieval/decryption and deadlines, including reports,
before expiry. Missing first receipts or unarchived logs remain acceptance gaps.

## Read-only retention audit

Select **`operation=audit-retention` explicitly** in
`test262-cutover-evidence.yml` on master. The existing protected environment and
OIDC role are reused under an intersecting temporary session policy limited to
bucket versioning, Object Lock, lifecycle and encryption reads, plus versioned
object metadata/retention. It grants no storage writes, listing or KMS decryption,
and changes no persistent permissions. S3 uses `GetObjectVersion` for both
versioned HEAD and GET; it is not a HEAD-only IAM permission. The reviewed audit
calls only HEAD/retention and never GETs a payload. The workflow's other operations
retain their separate guards; do not dispatch the default preservation operation.

The audit authenticates the accepted ledger and preservation evidence before any
S3 read, then checks the **41 exact archive versions plus the new receipt's exact
self-version** using HEAD and GetObjectRetention, never an unversioned/latest
object. It compares length, configured KMS identity, COMPLIANCE mode, current and
unshortened deadlines and known Actions expiry. Earlier byte hashes remain pinned
expectations, **not freshly verified hashes**. The private original eight-object
receipt still has no recorded self-version and remains explicitly unassessed.

Four independent configuration observations distinguish absent lifecycle
configuration from denied/invalid reads. Sanitized lifecycle rules retain only
enabled state, conservative destination-prefix overlap, expiration/noncurrent
expiration settings and transition counts. Rule IDs, prefixes, tags, keys, KMS/role
identifiers and raw diagnostics are not published. Tag/size conditions are
unassessed; transitions alone are not deletions. No lifecycle configuration is
not an indefinite-retention guarantee, and expired locks do not prove deletion.
IAM/KMS grants, replicas and restricted log contents are outside this audit.

Denied/invalid reads remain incomplete, independent reads continue, and the job
retains its sanitized partial report with a failing exit. Fresh exact-version
metadata findings take precedence over dated archive deadline findings in the
audit; the immutable historical ledger and nested historical coverage are not
rewritten. Existing backup/restore receipt collection uses GitHub only: no export,
restore, import, fixture workload, database access or notification is triggered.
Retain an authenticated public audit projection in Git before the 90-day Actions
copy expires; that mirror is not an independently locked raw receipt.

### October 26 read-only audit: metadata verified, lifecycle denied

[Run 40418650287](https://github.com/tomacox74/js2il/actions/runs/40418650287),
attempt 1, ran the reviewed `audit-retention` operation on exact master
`8aefbe00c59d2547c953c4aa19f6e1722d3e5206`, after central invariants
[40418215456](https://github.com/tomacox74/js2il/actions/runs/40418215456) passed.
The [authenticated sanitized receipt](evidence/retention-audit-40418650287.json)
has SHA-256 `acc6935ffa8d0c3e2d63724ba6410ee2ca6f856eed865c16d63ee208474eb632`.

**The audit is incomplete, not healthy-by-default.** Fresh metadata verified all
**42 exact versions**: the accepted 41 archive versions plus the newer receipt's
self-version. Length, configured KMS identity, COMPLIANCE mode and current,
unshortened deadlines matched; all 41 archive deadlines are unchanged. No payload
hash was rechecked or decryption attempted. The private original eight-object
receipt's self-version remains unrecorded/unassessed.

Bucket versioning was enabled, the default was **35-day COMPLIANCE**, and default
encryption was the configured KMS identity. However, the existing reader was
denied **`s3:GetLifecycleConfiguration`**. This is unknown lifecycle behavior,
not evidence of no rules, no deletion or indefinite retention. Independent
observations continued; the job correctly failed with `audit_complete=false`.
No permission was expanded and no alternate privileged reader was used.

Forty-one objects have known Actions expirations beyond their observed locks;
one archive's source expiration remains unknown. **No beyond-Actions protection
is established.** Earliest archive lock expiry remains November 12; later
archive/new-receipt locks remain November 30. Expiry itself is not deletion.
Other preservation/readback/discovery operations were skipped, and no new backup,
restore, import, fixture workload, payload download, storage write, retention
extension, policy change or notification occurred.

The receipt also retains authenticated observations of existing daily backup
40266462184 and weekly restore 40165197393. Their receipts were successful and
within the adopted 36-hour/eight-day monitor windows at the observation. The older
weekly receipt lacks the new self-report and recovery-timing fields: first
scheduled post-implementation verification is still pending, not a measured zero
or an RPO/RTO acceptance. No replacement drill was dispatched.

The remaining read-only prerequisite is an owner-reviewed grant of only the
missing bucket lifecycle read to the existing reader, then another explicit
`audit-retention` observation. That grant is **not authorized or applied here**.
That observation is historical; the successful October 27 rerun below supersedes
its lifecycle-read prerequisite. Longer-term ownership and renewal remain separate
decisions.

### October 27 read-only rerun: lifecycle absence observed

After the owner supplied the missing read permission, explicit read-only
[run 40435377938](https://github.com/tomacox74/js2il/actions/runs/40435377938),
attempt 1, source `3d46b322f2c3cc97df5405fd313392c0401b3a87`,
completed with `audit_complete=true`. S3 returned
`NoSuchLifecycleConfiguration`, not access denial. All **42 exact versions**
matched the prior metadata observations without changed locks; versioning,
35-day COMPLIANCE default and configured KMS identity were unchanged. The
[authenticated compact observation](https://github.com/tomacox74/js2il/issues/2343#issuecomment-6431827427)
records report SHA-256
`9f7191db7e6654ec0e063935b9ce7daa963ae9736b5f078d6be2950af9f19e60`.
No payload read, retention extension or other write occurred. Lifecycle absence
is a dated observation, not an indefinite protection guarantee.

## Owner-authorized fixed-version retention extension

The owner separately authorized extending COMPLIANCE on the accepted **41 archive
versions and verified newer preservation receipt version** through
**2027-04-30T00:00:00Z**, followed by exact-version verification. This does not
approve uploads, deletions, imports, restores, production changes, permission
changes, bucket defaults/lifecycle/KMS changes or the broader draft policy below.
COMPLIANCE extensions cannot subsequently be shortened.

Select **`operation=extend-retention`** explicitly on master with
**`retention_authorization=extend-42-retention-20270430`**. The protected existing
OIDC role is restricted by a separate intersecting session policy to the audit's
six read actions plus `s3:PutObjectRetention`. No role or bucket permission is
changed; missing write permission is a blocker, not permission to grant it. The
token and target deadline are fixed in the reviewed collector, not arbitrary
version, destination or date selectors.

The collector authenticates both immutable evidence pins, checks configuration
and **all 42 exact versions before its first write**, and refuses mutation on any
unknown/invalid preflight. It PUTs only COMPLIANCE retention for the pinned
version IDs; already equal/longer deadlines are left unchanged. SDK retries are
disabled. Local sanitized progress is journaled before each irreversible request,
after acknowledgment and verification; no replacement object or S3 report is
uploaded. Each acknowledged PUT is immediately read back. The first denied,
ambiguous or unverifiable write stops subsequent writes, followed by a final
read-only check of all 42 versions. Reports distinguish attempts, acknowledgments
and actual verified protection, including partial outcomes; no rollback is
attempted. A failed operation stays failed even if readback resolves an ambiguous
request as applied.

Retain an authenticated sanitized success/partial/denial report in Git before the
90-day Actions report expires. Only successful exact-version observations prove
the resulting horizon; merging tooling is not proof of an extension. These are
metadata/length/KMS observations, not fresh payload rehash/decryption proofs. The
original private eight-object receipt remains outside this authorization because
its self-version is unrecorded. The original 11 historical gaps and #2343's
ownership, renewal, service-objective and recovery-verification gates remain.

### October 27 authorized attempt: retention PUT denied, locks unchanged

The guarded tooling from #2410 passed exact-master central invariants
[40437418376](https://github.com/tomacox74/js2il/actions/runs/40437418376).
The separately authorized
[extension run 40437767853](https://github.com/tomacox74/js2il/actions/runs/40437767853),
attempt 1, source `03d40ed9cbcd5d4ac01541a1a5a5df3224b4ea09`,
then completed its **42/42 metadata and configuration preflight**, including
known lifecycle absence. The first exact-version `s3:PutObjectRetention` request
returned **`AccessDenied`**. The collector stopped: **one PUT attempt, zero
acknowledgments, 41 targets not attempted, no retry or rollback**.

Final read-only verification still checked **all 42 exact versions**, with matching
length, configured KMS identity, COMPLIANCE mode and **all deadlines unchanged**.
**Zero versions were verified through the requested April 30, 2027 horizon.**
The earlier November 12 and newer November 30 deadlines still apply; protection
beyond Actions source expiry remains unproven. This is a denied extension with
successful unchanged-metadata verification, not a successful extension or missing
objects. Payload hashes were not rechecked.

The [authenticated sanitized result](evidence/retention-extension-40437767853.json)
has SHA-256
`10a674761615feb6bc7f34809c0ed927c117dcd54f42cb3c28996e8e44410d13`;
its exact Actions report has SHA-256
`d4637b3a76009a925f99121c1dc8bee93e4a5c14966188950b5e8943e6efaffc`.
The original Actions artifact expires January 25, 2027; this Git projection
preserves the compact denied-attempt and before/after observations, not a newly
locked S3 report.

An owner-reviewed permission decision for `s3:PutObjectRetention` on the authorized
versions was the execution blocker at that observation. The denial does not identify which
identity/resource policy or other restriction caused it. **No permission change
was authorized or made**, and no alternate privileged principal or repeat
mutation was used. No S3 uploads/deletions, payload reads, imports, restores,
fixtures, database/production changes, bucket/KMS/lifecycle changes or notifications
occurred. Broader #2343 policy/recovery gates and the original 11 gaps are unchanged.

### October 27 authorized retry: all 42 versions protected through April 30

After the owner confirmed adding the narrow IAM policy, explicit
[run 40445723674](https://github.com/tomacox74/js2il/actions/runs/40445723674),
attempt 1, source `968756594b68290acadb81f01336cc7c02454b99`,
completed successfully. Its retention code and workflow were unchanged from the
previous reviewed/exact-master-verified implementation; that intervening commit
only retained the earlier denied-attempt evidence.

All **42 authorized versions** passed complete configuration/metadata preflight,
then **42 exact-version PUTs were acknowledged**, immediately read back, and
finally verified together at **2026-10-27T03:43:45Z**. The **41 accepted archives
and verified newer preservation receipt** now each have COMPLIANCE retention
through **2027-04-30T00:00:00Z**. Version IDs, lengths, configured KMS identity and
unshortened COMPLIANCE proofs matched; no object version or payload was replaced.
Existing byte hashes remain pinned expectations, not fresh rehash/decryption proofs.

The [authenticated sanitized result](evidence/retention-extension-40445723674.json)
has SHA-256
`facd2a698d64137ca063ffdbf26903096370c2aeecc7f047728f8809853c3a59`.
Its exact Actions report has SHA-256
`d32ea46c4c651887f7bac23be4d2b5c2ba5f63e4851d7bb76ea9a4ab5cf246ad`;
the ZIP has SHA-256
`068d7233b5a06ece5864f9fa9d014da3e94027a5fbc4aa88c1f6e98c3adf74d5`.
Actions artifact `12708161865` expires January 25, 2027; Git retains the compact
authenticated projection, not a newly uploaded/locked S3 report.

The new horizon covers the frozen known Actions expirations of **40 archives and
the newer receipt**. One archive's source expiration remains unknown; do not infer
coverage beyond an unknown expiry. The November deadlines remain true in the
unchanged historical ledger and earlier denied-attempt record, but are **not the
latest recorded deadlines of these 42 versions**. Schema 3 coverage uses the
authenticated extension for latest-recorded findings while retaining the original
history, 11 gaps and unknown source expiry. A fresh live audit still takes
precedence over dated archive/receipt findings; independent backup/restore
failures remain visible.

No S3 object/report uploads, deletions, payload reads, imports, restores, fixtures,
database/production, bucket defaults/KMS/lifecycle or notification changes occurred.
The owner added the permission; the workflow made no permission changes. The
original private older receipt remains unassessed because its self-version is
unrecorded. This finite extension does not approve indefinite/180-day retention,
ongoing ownership/renewal, RPO/RTO, a new rehearsal or production cutover.

## Draft policy and accountable-owner decision

**Not adopted or implemented.** The following makes the remaining decision
concrete; merge authorization does not authorize irreversible lock extensions,
permission changes, new storage classes or notification routing. Existing daily
backups, weekly isolated verification and hourly dashboard-first monitoring stay
unchanged. The adopted #2341 dashboard warnings remain active with notifications
paused; no new alert destination or service-recovery objective is selected here.

| Class | Proposed horizon and verification | Accountable role / approval still required |
| --- | --- | --- |
| Irreplaceable original archives and recovery containers | Retain indefinitely as an organizational policy, with explicitly approved finite COMPLIANCE windows and renewal rather than an invented infinite S3 lock; weekly pinned metadata review and monthly approved exact-version hash/decrypt verification | Archive operator and independent verifier, #2343; assign primary/backup, budget, retrieval access and renewal responsibility |
| Compact provenance, receipts and accepted gap decisions | Indefinite sanitized Git mirror plus independently retrievable exact-version originals where available; capture each acceptance/drill and review receipt self-retention with its payload | Evidence owner, #2343; approve long-term destination/immutability and resolve the private original receipt's unknown self-version without assuming it lost |
| Catalogue dumps and manifests | Keep the existing daily backup and at least 35-day COMPLIANCE floor; review newest exact versions/receipt daily and weekly disposable verification; longer recovery/history horizon requires a separate decision | Backup/recovery operator, #2343; approve RPO/RTO and data-loss/replay boundaries; a daily schedule is not an RPO guarantee |
| Isolated verification reports | Retain each sanitized success/failure report under the approved evidence horizon; verify first scheduled post-implementation self-version and measured snapshot/restore fields | Recovery operator plus independent verifier, #2343; lack of those fields in an older scheduled receipt is unknown, not zero recovery time |
| Useful failed/partial outboxes and restricted diagnostics | Archive useful original evidence before Actions expiry with hashes, provenance and access classification; do not archive credentials or indiscriminately duplicate raw logs | Archive/monitoring operators, #2343/#2341; define useful contents, redaction, retrieval and deletion criteria |
| Verbose disposable logs | Proposed 180-day hot retention, then useful hashed archival; current Actions convenience remains 90 days | Monitoring owner, #2341 with #2343; approve cost, access, archive/delete criteria and a supported destination before changing retention |

**Separately approved finite horizon:** the owner authorized only the 42-version
extension through **2027-04-30T00:00:00Z** described above, beyond their known
January source/report Actions expirations. The successful retry and exact-version readback above establish that finite
horizon; authorization/tooling or the earlier denied attempt alone would not.
Unknown source expiry remains unknown. Record every resulting version, deadline
and receipt, then decide longer-term renewal separately. Under the historical
proofs, earlier archive locks begin expiring **November 12, 2026 UTC**; later
archive/new-receipt locks expire **November 30**. Those historical deadlines are
not rewritten, and lock expiry is not a deletion prediction.

For approval, record the named primary/backup and independent verifier, accepted
horizons/cost, renewal lead time and access separation. Coordinate warning thresholds
and notification destinations with #2341; do not treat a report finding as a
delivered alert. Proposed service **RPO <=24h / RTO <=4h** remain separate decisions.
Any new recovery/failure-injection rehearsal needs scoped approval of disposable
targets, data, phases and cleanup; production cutover/rollback remains gated below.

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

#2343 stays open for owner policy/role assignments and renewal planning before the
verified April 30 deadline, broader archive/receipt/log coverage and retrieval, and the first
post-review retained measurement/report verification,
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
This October 25 record remains unchanged as historical evidence. The later
owner-accepted October 26 ledger supersedes its selected-eight coverage handoff;
ongoing retention belongs to #2343, and #2242 remains closed.
