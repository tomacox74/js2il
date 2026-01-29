<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.10: Tail Position Calls

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

ECMA-262 defines static tail-position analysis (IsInTailPosition / HasCallInTailPosition) and a runtime hook (PrepareForTailCall) used for Proper Tail Calls (PTC). JS2IL currently emits calls normally and does not implement PTC/tail-call optimization.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.10 | Tail Position Calls | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-tail-position-calls) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.10.1 | Static Semantics: IsInTailPosition ( call ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isintailposition) |
| 15.10.2 | Static Semantics: HasCallInTailPosition | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hascallintailposition) |
| 15.10.3 | PrepareForTailCall ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall) |

## Support

Feature-level support tracking with test script references.

### 15.10.3 ([tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| proper tail calls (PTC) / tail-call optimization | Not Yet Supported |  | No tail-position analysis and no tail-call emission; calls compile to regular call/callvirt so recursion grows the .NET stack. |

