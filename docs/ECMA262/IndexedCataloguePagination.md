# Indexed catalogue pagination

Issue #2348 blocks the full scoped export verification in #2337. Generic
`api_read` pagination previously materialized filter/cursor records as relations.
PostgreSQL could scan the primary-key index from the beginning and apply the
cursor as a late join filter. Later export pages revisited preceding rows.

The additive migration
`supabase/migrations/20261022060000_test262_catalogue_indexed_read_cursors.sql`
replaces only `test262.api_read`. Typed scalar subqueries make filter and cursor
values initplans, allowing primary-key index conditions. Composite keys retain
constraint ordinal order. The ownership predicates, supported tables, input
validation, page limits, payload cap and indexed legacy-control branch remain.
No catalogue data, grants, role membership or authority settings are changed.
The exporter still uses one repeatable-read/read-only transaction, exports all
allowlisted tables and validates its snapshot/count/digest receipt.

## Validation

The disposable PostgreSQL 17 integration suite loads `auto_explain` and captures
the actual nested `api_read` query plans. A rollback-only test corpus contains
32,768 interleaved fixtures from two repositories and 49,152 composite-key
eligibility rows. Early and late pages must use cursor index conditions and
examine a bounded number of rows. Tests also cover composite-key boundary
continuation, typed equality filters, foreign-repository invisibility and invalid
filter/cursor/page rejection. Existing tests cover raw/untrusted write denial,
immutable metadata, empty annotation exports and coherent export receipts.
No synthetic rows are inserted into production.

## Deployment and operational acceptance

1. Review/merge the migration and wait for PostgreSQL integration CI to pass.
2. Before applying it, inspect the production migration history and current
   `api_read` definition. Confirm the deployed annotation allowlist and indexed
   legacy-control branch match this migration's baseline. Stop and reconcile
   any intervening API changes instead of overwriting them.
3. Apply only this new additive migration through the authorized Supabase
   operator, with the existing recovery point and short lock timeout. Do not
   replay old migrations, change privileges or run historical enrichment.
4. Dispatch **Test262 specification deployment verification** from `master`.
   Use the existing protected coordinator/reporter secrets. Do not extend the
   deadline, truncate the export, or relax repository scope to get a green run.
5. Retain both sanitized receipts and verify matching project references,
   repository/epoch, complete table counts (including the empty annotation
   table), SHA-256 and MVCC snapshot identity. Source payloads stay temporary.
6. Record the migration receipt and successful run in #2348 and #2337 before
   closing their operational acceptance items. A merged PR or passing disposable
   tests alone is not evidence of a completed production export.
