# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `b2eee3cb` (merge of PR #820 bounded canary smoke lane for issue #583)
- GitHub: open issues/PRs state as of 2026-03-08 (refreshed in-place)

## Current active item
**Issue #776** (OPEN; active work is now on `copilot/gh-776-object-invariants`, with draft PR #821 open for review):
- https://github.com/tomacox74/js2il/issues/776
- Draft PR: https://github.com/tomacox74/js2il/pull/821

Rationale:
- #583 is now closed via merged PR #820, so #776 becomes the next unfinished ranked medium-priority item from this list.
- The current branch tightens ordinary-object descriptor invariants and own-key ordering in `JavaScriptRuntime.Object`, `PropertyDescriptorStore`, and `ForInIterator`.
- Focused Object execution/generator coverage is green for descriptor redefinition invariants, non-extensible-object behavior, and own-key ordering, and the draft PR is now ready for review.
- This is a good dependency for the adjacent integrity follow-up in #777 because the seal/freeze/preventExtensions surface depends on accurate ordinary-object descriptor rules.

## Recommended next item after #776
**Issue #777** (OPEN):
- https://github.com/tomacox74/js2il/issues/777

Rationale:
- It is the next adjacent medium-priority spec follow-up once ordinary-object invariants land.
- It can build directly on the descriptor and own-key correctness work being done for #776.

## Recently completed since the previous snapshot
- **#583** (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) — CLOSED via PR #820 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/583
  - https://github.com/tomacox74/js2il/pull/820
- **#775** (priority:high) ECMA-262: RegExp modern flags and symbol methods parity — CLOSED via PR #819 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/775
  - https://github.com/tomacox74/js2il/pull/819
- **#774** (priority:high) ECMA-262: Complete %TypedArray% constructors + prototype methods — CLOSED via PR #818 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/774
  - https://github.com/tomacox74/js2il/pull/818
- **#773** (priority:high) ECMA-262: Implement ArrayBuffer + DataView primitives (foundation for TypedArray semantics) — CLOSED on 2026-03-07
  - https://github.com/tomacox74/js2il/issues/773
- **#581** (priority:high) Quality: bounded differential testing (Node vs JS2IL) for semantic regressions — CLOSED on 2026-03-06
  - https://github.com/tomacox74/js2il/issues/581
- **#584** (priority:high) Quality: add compiler shape-coverage micro-tests (joins/back-edges/materialization) — CLOSED on 2026-03-06
  - https://github.com/tomacox74/js2il/issues/584
- **#812** perf(abi): introduce callable scope ABI attribute and variants — merged via PR #813 on 2026-03-07
  - https://github.com/tomacox74/js2il/issues/812
  - https://github.com/tomacox74/js2il/pull/813

## Triage note (hygiene)
**Issue #772 is still OPEN but appears implemented in `master`** (commit `78edaae0` and later master updates). It should be closed or explicitly re-scoped:
- https://github.com/tomacox74/js2il/issues/772
- https://github.com/tomacox74/js2il/commit/78edaae01b2b3da4560068b12314005a4c40387a

## Top 10 open issues (excluding #772, which still looks like hygiene work)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #776 (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering)
   - https://github.com/tomacox74/js2il/issues/776
   - Current branch status: `copilot/gh-776-object-invariants` now has draft PR #821 open with runtime + focused test updates and refreshed ECMA-262 tracking docs.
2. #777 (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*)
   - https://github.com/tomacox74/js2il/issues/777
3. #778 (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction)
   - https://github.com/tomacox74/js2il/issues/778
4. #779 (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics)
   - https://github.com/tomacox74/js2il/issues/779
5. #780 (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator])
   - https://github.com/tomacox74/js2il/issues/780
6. #787 (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth)
   - https://github.com/tomacox74/js2il/issues/787
7. #788 (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes)
   - https://github.com/tomacox74/js2il/issues/788
8. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
   - https://github.com/tomacox74/js2il/issues/789
9. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
    - https://github.com/tomacox74/js2il/issues/790
10. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
    - https://github.com/tomacox74/js2il/issues/781

## Remaining open issues
11. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
    - https://github.com/tomacox74/js2il/issues/791
12. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
    - https://github.com/tomacox74/js2il/issues/792
13. #419 Hosting: support mutable CommonJS exports (Node-like)
    - https://github.com/tomacox74/js2il/issues/419
14. #439 Hosting: publish referenceable library/build NuGet package
    - https://github.com/tomacox74/js2il/issues/439
15. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
16. #727 Function length/name should be descriptor-backed own properties
    - https://github.com/tomacox74/js2il/issues/727
17. #728 Complete bound function semantics for constructor/new-target and metadata
    - https://github.com/tomacox74/js2il/issues/728
18. #737 perf: callsite-based typed parameter specialization for non-exported functions
    - https://github.com/tomacox74/js2il/issues/737
19. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
    - https://github.com/tomacox74/js2il/issues/738
20. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740
21. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
22. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
23. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
24. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
25. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
26. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #821 (DRAFT): https://github.com/tomacox74/js2il/pull/821 — ordinary object internal method invariants for issue #776.
- #704 (OPEN): https://github.com/tomacox74/js2il/pull/704 — earlier attempt at issue #583 canary smoke coverage; now superseded by merged PR #820.

## Label/metadata gaps (as of this snapshot)
- Open issues: 28
- Missing `priority:*` label: 14
- Missing `lane:*` label: 28
