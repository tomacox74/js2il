# Specification metadata deployment preflight — issue #2337

Status: **not deployed**. Preflight inspection found a production compatibility difference that must be preserved before applying the specification-reference migration.

## Source and target

- Requested source: `a909f8168460c51318209e0c82f91153f1295e67`.
- Requested migration: `20261021230000_test262_catalogue_specification_metadata.sql`.
- Target: selected JROC performance Supabase write connector; database `postgres`; catalogue repository `tomacox74/js2il` / `faf01df8-aa65-4375-a708-6c1e555f957b`.
- The connector did not expose a project reference through the inspected database settings; do not treat a guessed project ID as verified.
- Database inspection time: `2026-10-22T01:57:51.54093+00:00` (UTC).
- Contract: schema 1, API contract 1, active authority, minimum writer epoch 2.

## Blocker and prepared correction

Production migration history includes `20261006101949_test262_legacy_control_read_indexed_scope`. Production `api_read` has a special typed/indexable read branch for `legacy_control_records`. The requested migration recreated the older generic reader and would remove that branch. The production table has approximately 1.5 million legacy-control rows, making this an operational regression risk.

The correction retains the inspected production function definition and adds only `fixture_specification_metadata` to its supported-table list. Its legacy-control typed filters, ownership predicate, cursor ordering and ordinary reader behavior are preserved. The API subject/authority model is unchanged. The original `api_put` and `api_scope` changes from #2335 remain intact. No production DDL or data writes were performed during this preflight.

Preflight production function-body MD5s: `api_read=5f114a6e2e731afeb23a7fd3af415302`, `api_put=f0b563e892c67d3b98bdbdb754da3052`, `api_scope=829ce5cf85dd0faa20424f6bff4a02d3`. Recheck before deployment and inspect any intervening drift.

## Existing recovery evidence

The scheduled catalogue-only backup [run 39540754977](https://github.com/tomacox74/js2il/actions/runs/39540754977) succeeded. Its log recorded `backup_verified=true` and receipt version `wUDCa9uPGzMo7DczIJpiP.ObUzXYrTJk` at `2026-10-21T03:53:25Z`. Sanitized report artifact ID: `12319546992`, artifact ZIP SHA-256: `f5979958779d040141811347b611a97f45a5b54870f9f2e700ee0b8a0e2c129d`. This is existing preflight backup evidence, not a fresh backup or a completed post-deployment restore drill.

The backup workflow dumps both `test262` and `test262_reporting` schemas. The existing `test262_backup` login has catalogue SELECT grants and BYPASSRLS. After the new objects exist, it needs narrowly scoped SELECT for the annotation table and reporting view. No grant was applied during preflight. Do not broaden performance-data or public Data API access.

## Remaining deployment work

1. Validate/review the corrected migration; retain the final source SHA.
2. Preserve an approved pre-deployment recovery point and verify the exact target project.
3. Apply the additive migration using the authorized migration connector, once; retain its receipt and server timestamp.
4. Extend only the existing catalogue backup identity's SELECT allowlist for the two new objects.
5. Verify guards, scoped reads, reporter view, original provenance, unresolved/unavailable old inventories and unattempted/null interpretation fields.
6. Exercise mutation rejection in a disposable PostgreSQL instance, not with synthetic production fixtures.
7. Retain a coherent scoped NDJSON export and post-deployment backup/restore evidence.
8. Compare authority/epoch, protected historical evidence and reporting targets; allow for independently running ingestion rather than attributing concurrent counts to this deployment.

Historical enrichment remains excluded. It must wait for #2336 and an immutable-annotation compatibility plan. Issue #2337 remains open until its deployment acceptance criteria are evidenced.
