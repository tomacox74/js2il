<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.10: Tail Position Calls

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-25T20:32:47Z

ECMA-262 defines static tail-position analysis (IsInTailPosition / HasCallInTailPosition) and a runtime hook (PrepareForTailCall) used for Proper Tail Calls (PTC). JROC currently emits calls normally and does not implement PTC/tail-call optimization.

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

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.10.3 ([tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| proper tail calls (PTC) / tail-call optimization | Not Yet Supported | [`tco-pos.js`](../../../tests/Jroc.Test262.Tests/language/expressions/JavaScript/tco-pos.js)<br>[`tco-member-args.js`](../../../tests/Jroc.Test262.Tests/language/expressions/call/JavaScript/tco-member-args.js)<br>[`tco-stmt.js`](../../../tests/Jroc.Test262.Tests/language/statements/block/JavaScript/tco-stmt.js)<br>[`tco-stmt-list.js`](../../../tests/Jroc.Test262.Tests/language/statements/block/JavaScript/tco-stmt-list.js)<br>[`tco-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/do-while/JavaScript/tco-body.js)<br>[`tco-const-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-const-body.js)<br>[`tco-let-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-let-body.js)<br>[`tco-lhs-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-lhs-body.js)<br>[`tco-var-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-var-body.js)<br>[`tco-else-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/tco-else-body.js)<br>[`tco-if-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/tco-if-body.js)<br>[`tco.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/tco.js)<br>[`tco.js`](../../../tests/Jroc.Test262.Tests/language/statements/return/JavaScript/tco.js)<br>[`tco-case-body-dflt.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-case-body-dflt.js)<br>[`tco-case-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-case-body.js)<br>[`tco-dftl-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-dftl-body.js)<br>[`tco-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/while/JavaScript/tco-body.js) | `test/language/expressions/tco-pos.js`<br>`test/language/expressions/call/tco-member-args.js`<br>`test/language/statements/block/tco-stmt.js`<br>`test/language/statements/block/tco-stmt-list.js`<br>`test/language/statements/do-while/tco-body.js`<br>`test/language/statements/for/tco-const-body.js`<br>`test/language/statements/for/tco-let-body.js`<br>`test/language/statements/for/tco-lhs-body.js`<br>`test/language/statements/for/tco-var-body.js`<br>`test/language/statements/if/tco-else-body.js`<br>`test/language/statements/if/tco-if-body.js`<br>`test/language/statements/labeled/tco.js`<br>`test/language/statements/return/tco.js`<br>`test/language/statements/switch/tco-case-body-dflt.js`<br>`test/language/statements/switch/tco-case-body.js`<br>`test/language/statements/switch/tco-dftl-body.js`<br>`test/language/statements/while/tco-body.js` | The ported tests cover tail-position call semantics across expressions and statements. JROC still emits regular call/callvirt instructions rather than PrepareForTailCall, so recursive tail calls grow the .NET stack. |

