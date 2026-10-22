# Single-pass catalogue export

The 30-minute production verification after #2360 still timed out: [39835961685](https://github.com/tomacox74/js2il/actions/runs/39835961685), source `a9ccc52fa97646a5cdf5079f86e0829057cb89fe`. Its checked coordinator artifact `12451538858`, ZIP SHA-256 `666d784594305cfb51ac780e9c16e2d48594a794707ca3c628881fbe4f331a90`, records `verified=false`, `TimeoutError` and `export_budget_seconds=1800`. It reached only 265,000 of 1,525,377 legacy control records after completing the earlier tables. The annotation canary and retained recovery checks passed; this export did not.

Repeated small LIMIT/keyset queries favor quick first pages and repeatedly perform planning, row conversion, JSON aggregation and heap lookups. They remain appropriate for interactive reads, but are a poor full-export execution path once the catalogue exceeds cache. The new bulk path opens one scoped scan per table and retains its position while the client consumes bounded batches. It avoids whole-table sorting and per-page JSON aggregation. Performance acceptance still requires a complete production receipt; there is no further deadline increase.

## Contract and boundaries

- `api_contract()` advertises additive `bulk_export_version=1`; schema/API version remains 1. Older deployments and nontrusted/noncoordinator clients keep the existing paged export path. Errors from an advertised bulk endpoint propagate instead of silently retrying another path.
- `api_export_open(epoch, table)` is executable only by the existing coordinator role. It requires an enabled, login-bound trusted coordinator, the current epoch, and a read-only repeatable-read or serializable transaction. The exact 32-table allowlist and typed repository ownership predicates are equivalent to `api_read`/`api_scope`.
- Each call creates an unpredictable, non-scrollable, non-holdable portal. The client adopts it on the same connection and transaction, fetching at most 1,000 rows, or 25 payload rows, at a time. Portals are explicitly closed on completion, early exit and deadline failure; transaction end also releases them.
- No raw-table access, writer privilege, public/Supabase API exposure, interactive page bound, payload bound, per-statement timeout or authority epoch changes. The 1,800-second full-export deadline and 35-minute verification job ceiling remain in place.

## Row order and receipts

The NDJSON header, table/row records, table set, footer counts and SHA-256 format are unchanged. **Bulk rows are unordered.** Consumers must join by repository and primary keys, never file position. Interactive `api_read` keeps its primary-key ordering and cursor semantics. A single-pass portal does not revisit pages, and ownership `EXISTS` predicates cannot multiply rows; complete row-set parity is tested against the existing helper for all 32 tables, including foreign and shared corpora.

All tables share the export's original read-only MVCC snapshot. The footer hashes the actual complete byte sequence; the independent verifier checks exact counts, digest, identity and snapshot. A missing footer, mismatch, timeout or cancelled read remains a failure, with no completed output promoted. The temporary NDJSON is deleted by deployment verification; only its sanitized receipt is retained.

## Validation and deployment

Disposable PostgreSQL tests cover all-table scope parity, interleaved foreign records at scale, single scan execution across multiple bounded fetches, trusted-role/epoch/transaction/allowlist denial, a concurrent insert excluded from later tables, and portal cleanup with no completion footer after a deadline. Unit tests cover payload/page bounds, lazy fetching, early close, compatibility and propagated authorization failures. CI also runs the existing indexed reader, write API and worker pilot checks.

After review/merge, apply only the guarded additive migration, check the installed function/contract bodies and unchanged writer/reader/authority/data fingerprints, then rerun protected production verification with no concurrent backup/preflight. #2358 stays open until that complete receipt passes. Historical corpus expansion remains paused until then.

Implementation references: [Psycopg server cursors](https://www.psycopg.org/psycopg3/docs/advanced/cursors.html#stealing-an-existing-cursor) and [PostgreSQL 17 cursor planning](https://www.postgresql.org/docs/17/runtime-config-query.html#GUC-CURSOR-TUPLE-FRACTION).
