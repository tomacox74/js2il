# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and a single recommended “next item to fix”.

Synced to:
- Repo: `master` @ `78edaae0` (fix(esm): complete issue #772)
- GitHub: open issues/PRs state as of 2026-03-06

## Recommended next item
**Merge PR #704** (OPEN; non-draft):
- https://github.com/tomacox74/js2il/pull/704

Rationale:
- Lands the canary smoke gate (fixes #583) and improves ecosystem stability.
- It’s a contained change set vs starting a new runtime/spec feature.

## Triage note (hygiene)
**Issue #772 is still OPEN but appears implemented on `master`** (commit `78edaae0` “fix(esm): complete issue #772 ...”). If verified, close the issue:
- https://github.com/tomacox74/js2il/issues/772
- https://github.com/tomacox74/js2il/commit/78edaae01b2b3da4560068b12314005a4c40387a

## Top 10 open issues (excluding #772, which appears completed in `master`)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #581 (priority:high) Quality: bounded differential testing (Node vs JS2IL) for semantic regressions
   - https://github.com/tomacox74/js2il/issues/581
2. #584 (priority:high) Quality: add compiler shape-coverage micro-tests (joins/back-edges/materialization)
   - https://github.com/tomacox74/js2il/issues/584
3. #773 (priority:high) ECMA-262: Implement ArrayBuffer + DataView primitives (foundation for TypedArray semantics)
   - https://github.com/tomacox74/js2il/issues/773
4. #774 (priority:high) ECMA-262: Complete %TypedArray% constructors + prototype methods
   - https://github.com/tomacox74/js2il/issues/774
5. #775 (priority:high) ECMA-262: RegExp modern flags and symbol methods parity (u/y/d/v, @@match, etc.)
   - https://github.com/tomacox74/js2il/issues/775
6. #583 (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) for ecosystem stability
   - https://github.com/tomacox74/js2il/issues/583
7. #776 (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering)
   - https://github.com/tomacox74/js2il/issues/776
8. #777 (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*)
   - https://github.com/tomacox74/js2il/issues/777
9. #778 (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction)
   - https://github.com/tomacox74/js2il/issues/778
10. #779 (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics)
    - https://github.com/tomacox74/js2il/issues/779

## Next 20 open issues
11. #780 (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator])
    - https://github.com/tomacox74/js2il/issues/780
12. #787 (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth)
    - https://github.com/tomacox74/js2il/issues/787
13. #788 (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes)
    - https://github.com/tomacox74/js2il/issues/788
14. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
    - https://github.com/tomacox74/js2il/issues/789
15. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
    - https://github.com/tomacox74/js2il/issues/790
16. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
    - https://github.com/tomacox74/js2il/issues/781
17. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
    - https://github.com/tomacox74/js2il/issues/791
18. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
    - https://github.com/tomacox74/js2il/issues/792
19. #419 Hosting: support mutable CommonJS exports (Node-like)
    - https://github.com/tomacox74/js2il/issues/419
20. #439 Hosting: publish referenceable library/build NuGet package
    - https://github.com/tomacox74/js2il/issues/439
21. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
22. #727 Function length/name should be descriptor-backed own properties
    - https://github.com/tomacox74/js2il/issues/727
23. #728 Complete bound function semantics for constructor/new-target and metadata
    - https://github.com/tomacox74/js2il/issues/728
24. #737 perf: callsite-based typed parameter specialization for non-exported functions
    - https://github.com/tomacox74/js2il/issues/737
25. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
    - https://github.com/tomacox74/js2il/issues/738
26. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740
27. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
28. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
29. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
30. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747

## Remaining open issues
31. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
32. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #704 (OPEN): https://github.com/tomacox74/js2il/pull/704
- #702 (DRAFT): https://github.com/tomacox74/js2il/pull/702
- #703 (DRAFT): https://github.com/tomacox74/js2il/pull/703

## Label/metadata gaps (as of this snapshot)
- Open issues: 33
- Missing `priority:*` label: 14
- Missing `lane:*` label: 33
