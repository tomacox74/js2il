# Full-catalogue verification budget

The protected specification-deployment verification exports every one of the 32 `EXPORT_TABLES` through the existing scoped API in one read-only repeatable-read snapshot. It independently checks the complete NDJSON file, exact table counts, SHA-256, repository, epoch, schema/API and snapshot identity before reporting success. The file is deleted; only a sanitized receipt is uploaded.

## Production measurement, 2026-10-22

After #2359's typed ownership predicates were deployed, the isolated run [39797715258](https://github.com/tomacox74/js2il/actions/runs/39797715258) on `57565ea86d26ebf6d07da751ccd8feb6b5b6ad19` still reached its 1,200-second deadline. No production backup or preflight overlapped that run. Eligibility completed all 5,022,608 records in about 486 seconds; it then reached 1,295,000 registrations before timing out. This is an incomplete export, never a successful receipt. Artifact `12436610279`, checked ZIP SHA-256 `0bdfc890f7226694f6a08ce95a14d2d46df6f5757a7d87f8961bb9d2a4810ff5`, records `verified=false / error_class=TimeoutError`.

The exact retained restore [39796617754](https://github.com/tomacox74/js2il/actions/runs/39796617754) independently counted 10,869,336 records across those 32 export tables at its backup snapshot, including 267,160 fixtures, 53,432 annotations, 5,022,608 eligibility records and 2,354,088 registrations. These are restored snapshot counts, not proof that the failed NDJSON export was complete.

A bounded production eligibility-page probe examined 1,000 indexed rows without filtering other rows: 727.803 ms with 629 disk blocks read, then 11.324 ms with no disk reads on repetition. Eligibility table/index sizes were 764,534,784 / 455,180,288 bytes versus 224 MB shared buffers. Those measurements support I/O sensitivity; removing backup overlap helped but did not solve the full-export deadline. The typed scope/index plan tests and all authorization checks remain required.

## Bounded offline verification

The coordinator's full-export budget is now **1,800 seconds (30 minutes)**. The workflow job has a **35-minute ceiling** to allow setup, independent file verification and receipt upload. The sanitized coordinator receipt states `export_budget_seconds`, including on failure. This is an explicit change to the earlier 20-minute operational acceptance budget; #2358 remains open until a complete export actually passes.

This changes no API grants, identity/trust checks, table scope, primary-key pagination, maximum page size of 1,000, payload-page cap of 25, per-statement 120-second timeout, read-only snapshot semantics, writer behavior or receipt integrity checks. The reporter's read checks are unchanged. A timeout, missing footer, wrong identity, count mismatch or digest mismatch remains a failure. Run this verification after the production backup/preflight has completed; disposable restore verification uses its own database.
