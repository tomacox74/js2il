<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.10: Tail Position Calls

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-24T09:22:49Z

JROC implements ECMA-262 tail-position analysis and proper tail calls for strict ordinary functions. Eligible calls transfer through a stack-safe runtime trampoline after evaluating the callee, receiver, and arguments. Protected calls remain in their caller when catch, finally, or IteratorClose semantics require it.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.10 | Tail Position Calls | Supported | [tc39.es](https://tc39.es/ecma262/#sec-tail-position-calls) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.10.1 | Static Semantics: IsInTailPosition ( call ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-isintailposition) |
| 15.10.2 | Static Semantics: HasCallInTailPosition | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hascallintailposition) |
| 15.10.3 | PrepareForTailCall ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.10.3 ([tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| proper tail calls (PTC) / tail-call optimization | Supported | [`tco-pos.js`](../../../tests/Jroc.Test262.Tests/language/expressions/JavaScript/tco-pos.js)<br>[`tco-call-args.js`](../../../tests/Jroc.Test262.Tests/language/expressions/call/JavaScript/tco-call-args.js)<br>[`tco-member-args.js`](../../../tests/Jroc.Test262.Tests/language/expressions/call/JavaScript/tco-member-args.js)<br>[`tco-pos-null.js`](../../../tests/Jroc.Test262.Tests/language/expressions/coalesce/JavaScript/tco-pos-null.js)<br>[`tco-pos-undefined.js`](../../../tests/Jroc.Test262.Tests/language/expressions/coalesce/JavaScript/tco-pos-undefined.js)<br>[`tco-final.js`](../../../tests/Jroc.Test262.Tests/language/expressions/comma/JavaScript/tco-final.js)<br>[`tco-call.js`](../../../tests/Jroc.Test262.Tests/language/expressions/tagged-template/JavaScript/tco-call.js)<br>[`tco-member.js`](../../../tests/Jroc.Test262.Tests/language/expressions/tagged-template/JavaScript/tco-member.js)<br>[`tco-stmt.js`](../../../tests/Jroc.Test262.Tests/language/statements/block/JavaScript/tco-stmt.js)<br>[`tco-stmt-list.js`](../../../tests/Jroc.Test262.Tests/language/statements/block/JavaScript/tco-stmt-list.js)<br>[`tco-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/do-while/JavaScript/tco-body.js)<br>[`tco-const-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-const-body.js)<br>[`tco-let-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-let-body.js)<br>[`tco-lhs-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-lhs-body.js)<br>[`tco-var-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-var-body.js)<br>[`tco-else-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/tco-else-body.js)<br>[`tco-if-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/tco-if-body.js)<br>[`tco.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/tco.js)<br>[`tco.js`](../../../tests/Jroc.Test262.Tests/language/statements/return/JavaScript/tco.js)<br>[`tco-case-body-dflt.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-case-body-dflt.js)<br>[`tco-case-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-case-body.js)<br>[`tco-dftl-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-dftl-body.js)<br>[`tco-catch.js`](../../../tests/Jroc.Test262.Tests/language/statements/try/JavaScript/tco-catch.js)<br>[`tco-catch-finally.js`](../../../tests/Jroc.Test262.Tests/language/statements/try/JavaScript/tco-catch-finally.js)<br>[`tco-finally.js`](../../../tests/Jroc.Test262.Tests/language/statements/try/JavaScript/tco-finally.js)<br>[`tco-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/while/JavaScript/tco-body.js) | `test/language/expressions/tco-pos.js`<br>`test/language/expressions/call/tco-call-args.js`<br>`test/language/expressions/call/tco-member-args.js`<br>`test/language/expressions/coalesce/tco-pos-null.js`<br>`test/language/expressions/coalesce/tco-pos-undefined.js`<br>`test/language/expressions/comma/tco-final.js`<br>`test/language/expressions/tagged-template/tco-call.js`<br>`test/language/expressions/tagged-template/tco-member.js`<br>`test/language/statements/block/tco-stmt.js`<br>`test/language/statements/block/tco-stmt-list.js`<br>`test/language/statements/do-while/tco-body.js`<br>`test/language/statements/for/tco-const-body.js`<br>`test/language/statements/for/tco-let-body.js`<br>`test/language/statements/for/tco-lhs-body.js`<br>`test/language/statements/for/tco-var-body.js`<br>`test/language/statements/if/tco-else-body.js`<br>`test/language/statements/if/tco-if-body.js`<br>`test/language/statements/labeled/tco.js`<br>`test/language/statements/return/tco.js`<br>`test/language/statements/switch/tco-case-body-dflt.js`<br>`test/language/statements/switch/tco-case-body.js`<br>`test/language/statements/switch/tco-dftl-body.js`<br>`test/language/statements/try/tco-catch.js`<br>`test/language/statements/try/tco-catch-finally.js`<br>`test/language/statements/try/tco-finally.js`<br>`test/language/statements/while/tco-body.js` | Strict ordinary functions use a stack-safe trampoline for eligible ordinary and tagged-template calls, including dynamic callees, member receivers, spread/rest arguments, closures, conditional/logical/nullish/comma expressions, and returns from eligible catch/finally handlers. Calls remain synchronous in protected regions when caller-side exception handling, finally execution, or IteratorClose must occur after the call. |

