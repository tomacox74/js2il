# Specification deployment verification

Follow-up tooling for #2337. Run `Test262 specification deployment verification`
on `master` after review/merge. The existing `test262-catalogue` environment's
master-only protection is retained. No schema, grants or credentials are changed.

The coordinator job uses the existing `TEST262_COORDINATOR_DATABASE_URL` and
repository/producer/epoch variables. It performs a three-row annotation API read,
checks raw INSERT privilege is absent, checks a foreign-repository filtered read
returns no rows, and runs the existing coherent scoped exporter with a 20-minute
deadline. Export bytes, footer counts, repository/epoch and digest are independently
checked before the temporary NDJSON is removed. Only the sanitized JSON receipt
is uploaded; export payloads and connection settings are never uploaded.

The reporter job requires `TEST262_REPORTER_DATABASE_URL` containing the existing
dedicated `test262_agent_ro` login, in that environment (or an existing repository
secret). It must be configured by the credential owner; missing credentials fail
closed. Do not substitute the coordinator/operator login or broaden membership.
It opens a read-only transaction, reads at most three reporting rows, and checks
the reporter lacks raw INSERT privilege. Receipts include the Supabase project
reference derived from the connection's host/pooler user, never its full DSN.

Compare the project references across the two retained receipts and the intended
production deployment receipt before checking off project identity in #2337.
This workflow does not test production mutations. Existing disposable PostgreSQL
integration tests remain the evidence for rejected cross-repository/untrusted
writes and immutable/source-identity guards. Missing or failed receipts are not
acceptance evidence. The existing backup/retained-restore workflows separately
verify recovery. Keep #2337 open until all acceptance checks have actual results.
