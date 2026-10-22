# Historical specification metadata canary (#2338)

The master-only `test262-specification-backfill.yml` workflow uses the existing
protected `test262-catalogue` environment and dedicated trusted coordinator.
It does not execute upstream JavaScript, allocate work, publish results, change
authority, migrate the schema or grant access. The only permitted writes are
source-verified immutable specification annotations for one selected corpus.

The default mode is read-only `preflight`. `apply` first completes the same
preflight, writes one bounded batch, deliberately stops after its acknowledgement,
then resumes the existing enrichment command and replays the complete corpus.
The final check compares annotation content, actors and timestamps to prove that
replay did not rewrite evidence. A failed run can be repeated with identical
inputs; conflicting annotations stop before further writes.

## Frozen source and scope

The reviewed parser fixes are merged in #2347 and #2354. At tooling revision
`e86bbf56f6cad390362b781b094ccc4b9ea3a91d`, a clean complete checkout of
Test262 `2b2ecead6e828dd9af13a9ec72065e645724a50f` produced:

- 53,432 unique fixtures, all path/content hashes checked.
- Source manifest SHA-256:
  `70a161c0a68617c93eeee60f7acd73ad6bb60ac230e2a063711f108dda41a4e6`.
- Corrected metadata manifest SHA-256:
  `f907651905082f70cc1bcf3fa2fe4ca42a6e761f7f6d34fbcfa09af695199d3e`.
- Reference states: 676 absent, 1,377 ambiguous, 9,864 legacy, 41,515 unresolved.
- 378 extraction diagnostics remain explicit; references are not invented.

Both manifests concatenate `path:lowercase_sha256\n` in path order, then hash the
UTF-8 bytes. Metadata entries use SHA-256 of the existing annotation metadata
projection and PostgreSQL-compatible canonical number formatting. These are
extraction fingerprints, not conformance scores or current-edition clause maps.

Start with corpus `cf3b4d8e-e508-5b39-94a6-dd455585be9e`, the oldest stripped
inventory. The workflow allows only the four reviewed corpus IDs and checks each
original inventory digest/revision/count. Other corpora require their own runs;
do not infer historical coverage from a successful canary.

## Recovery and execution

1. Run the existing catalogue off-project backup workflow on master. It must
   complete successfully on its first attempt and retain its sanitized report.
2. Select that numeric `backup_run` in the canary dispatch. Its workflow path,
   master branch, success and attempt are checked before downloading its report.
   The runner additionally requires a catalogue-only, independently retained,
   verified recovery receipt no older than six hours, with retained object IDs.
3. Set `metadata_digest` to the corrected manifest above, select the intended
   corpus and run `preflight`. Inspect its sanitized plan and historical hashes.
4. Run `apply` for the same reviewed corpus/digest/recovery point. It repeats
   source preflight and identity checks before any annotation batch.
5. Inspect the retained sanitized receipt. It must verify complete annotation
   counts, interrupted-batch resume, identical replay, and unchanged historical
   fixture/corpus/observation/payload fingerprints. New observations added by
   concurrent ingestion are allowed; mutation or removal of baseline identities
   fails verification. Pre-existing annotations and unexpected new annotation
   identities are also checked.
6. Independently inspect the reporting view for this repository and corpus, then
   retain a full coherent export through the existing deployment-verification
   workflow and a fresh post-write catalogue backup. Record receipts in #2338
   before expanding scope or marking its acceptance complete.

Only summaries, source/sample hashes and counts are uploaded. Source payloads,
connection settings and exception messages are excluded. The pre-write receipt
is saved before writes so failure retains the historical fingerprints. Repository
scope, epoch and trusted login checks remain enforced by the existing API.
