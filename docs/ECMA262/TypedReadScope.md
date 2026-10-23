# Typed catalogue read scope (#2358)

Full export 39778214229 reached the original 1,200-second deadline after the
successful #2338 canary. Progress counts are not a completed NDJSON receipt.
The canary, unchanged-history checks, reporter, retained backup and exact restore
passed.

The additive migration replaces only generic api_read predicates that call
api_scope(table, to_jsonb(x), repository). It uses equivalent typed EXISTS checks
over corpus/fixture/parent ownership relations, exposing them to the planner and
avoiding serialization of source metadata solely for authorization. Direct
repository-column predicates and the indexed legacy-control branch are unchanged.

Write authorization, API identity/trust/epoch checks, allowlists, parameter
binding, typed filters/cursors, payload cap 25, maximum page 1,000, ordering and
single read-only repeatable-read export semantics remain unchanged. No data
writes, grants, authority changes or new security-definer entry points are added.

PostgreSQL regression compares all 32 exported tables with the existing helper,
including foreign fixtures/variants/dependencies/annotations and a legitimately
shared corpus. Actual nested API plans at 32,768-row interleaved foreign scale
check early/late fixture, variant, dependency and composite eligibility cursors,
bounded examined rows and absence of per-row api_scope calls. Existing raw-write,
untrusted identity, immutable replay, exact cursor/filter, count/digest/snapshot
and worker/restore tests remain required.

Apply only 20261022130000_test262_catalogue_typed_read_scope.sql after PR checks.
Recheck the selected production reader definition before applying: MD5 of
pg_get_functiondef was 6b65cadaf1afa5838a6f956e2ae7ded4 at
2026-10-22T12:48:32.547384Z. Guard repository identity, active schema/API 1 and
epoch 2, and use a five-second lock timeout. Stop on intervening reader changes.

Retain recovery prefix 2026/10/22/39778132330-1; its exact disposable restore
passed with 53,432 annotations. Verify only api_read changed, annotation/history/
inventory fingerprints and performance-data permissions remain unchanged.
Rerun the protected export within the original deadline, retaining its actual
independently checked all-table counts/digest/snapshot receipt before acceptance.
