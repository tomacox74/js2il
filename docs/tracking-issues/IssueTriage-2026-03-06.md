# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and a single recommended “next item to fix”.

## Recommended next item
**Merge PR #704** (READY; checks green):
- https://github.com/tomacox74/js2il/pull/704

Rationale:
- It’s already implemented + validated by CI, so landing it is high-leverage vs starting a new feature.
- Improves ecosystem stability via a real-world canary smoke gate.

## Triage note (hygiene)
**Issue #787 is still OPEN even though it was merged via PR #804**. It should be closed as done:
- https://github.com/tomacox74/js2il/issues/787

## Top 10 open issues (excluding #787)
(Heuristic ranking using priority labels + lane keywords in titles/labels; recommendation-only.)

1. #772 (priority:high) ES Modules live bindings/module records
   - https://github.com/tomacox74/js2il/issues/772
2. #581 (priority:high) differential testing harness (Node vs JS2IL)
   - https://github.com/tomacox74/js2il/issues/581
3. #775 (priority:high) modern RegExp flags + symbol methods parity
   - https://github.com/tomacox74/js2il/issues/775
4. #774 (priority:high) complete `%TypedArray%` constructors + prototype methods
   - https://github.com/tomacox74/js2il/issues/774
5. #773 (priority:high) ArrayBuffer + DataView primitives
   - https://github.com/tomacox74/js2il/issues/773
6. #584 (priority:high) compiler shape-coverage micro-tests
   - https://github.com/tomacox74/js2il/issues/584
7. #790 (priority:medium) node crypto minimum subset
   - https://github.com/tomacox74/js2il/issues/790
8. #789 (priority:medium) node url/querystring baseline
   - https://github.com/tomacox74/js2il/issues/789
9. #788 (priority:medium) child_process beyond sync
   - https://github.com/tomacox74/js2il/issues/788
10. #780 (priority:medium) Array iterator methods
   - https://github.com/tomacox74/js2il/issues/780

## Next 20 open issues
11. #779 (priority:medium) Symbol ecosystem completeness audit
    - https://github.com/tomacox74/js2il/issues/779
12. #778 (priority:medium) Function constructor (`new Function`)
    - https://github.com/tomacox74/js2il/issues/778
13. #777 (priority:medium) Object integrity APIs semantics audit
    - https://github.com/tomacox74/js2il/issues/777
14. #776 (priority:medium) Ordinary object internal method invariants
    - https://github.com/tomacox74/js2il/issues/776
15. #583 (priority:medium) real-world canary corpus smoke tests (bounded)
    - https://github.com/tomacox74/js2il/issues/583
16. #792 (priority:low) http/https/net/tls baseline plan
    - https://github.com/tomacox74/js2il/issues/792
17. #791 (priority:low) ESM interop baseline plan
    - https://github.com/tomacox74/js2il/issues/791
18. #781 (priority:low) WeakRef + FinalizationRegistry host cleanup model
    - https://github.com/tomacox74/js2il/issues/781
19. #738 perf(prime): close PrimeJavaScript gap
    - https://github.com/tomacox74/js2il/issues/738
20. #737 perf: callsite-based typed parameter specialization
    - https://github.com/tomacox74/js2il/issues/737
21. #419 Hosting: mutable CommonJS exports
    - https://github.com/tomacox74/js2il/issues/419
22. #768 Perf: devirtualize calls to const/arrow bindings
    - https://github.com/tomacox74/js2il/issues/768
23. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
24. #748 perf(dispatch): RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
25. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
26. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
27. #742 perf(prime): trim timing/config coercion overhead
    - https://github.com/tomacox74/js2il/issues/742
28. #740 perf(prime): keep sieve loop math in typed locals
    - https://github.com/tomacox74/js2il/issues/740
29. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
30. #728 Bound function semantics for constructor/new-target/metadata
    - https://github.com/tomacox74/js2il/issues/728

## Open PRs (for context)
- #704 (READY): https://github.com/tomacox74/js2il/pull/704
- #702 (DRAFT): https://github.com/tomacox74/js2il/pull/702
- #703 (DRAFT): https://github.com/tomacox74/js2il/pull/703

## Label/metadata gaps (as of this snapshot)
- Open issues: 33
- Missing `priority:*` label: 14
- Missing lane categorization (heuristic): 17
