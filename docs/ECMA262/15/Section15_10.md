<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.10: Tail Position Calls

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-17T03:27:32Z

ECMA-262 defines static tail-position analysis (IsInTailPosition / HasCallInTailPosition) and a runtime hook (PrepareForTailCall) used for Proper Tail Calls (PTC). JROC can emit .NET tail calls for eligible statically resolved functions, including final comma operands and nullish-coalescing right operands with the object-returning ABI. General PTC for runtime-dispatched and tagged-template calls is not supported.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.10 | Tail Position Calls | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tail-position-calls) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.10.1 | Static Semantics: IsInTailPosition ( call ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isintailposition) |
| 15.10.2 | Static Semantics: HasCallInTailPosition | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hascallintailposition) |
| 15.10.3 | PrepareForTailCall ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.10.3 ([tc39.es](https://tc39.es/ecma262/#sec-preparefortailcall))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Runtime-dispatched and tagged-template proper tail calls | Not Yet Supported |  | `test/language/expressions/call/tco-call-args.js`<br>`test/language/expressions/tagged-template/tco-call.js`<br>`test/language/expressions/tagged-template/tco-member.js` | Runtime-dispatched calls and tagged-template invocations still grow the .NET stack. These three pinned expression cases overflow at the upstream recursion depth and are not included in verified passing coverage. |
| Statically resolved function tail calls | Supported with Limitations | [`tco-pos.js`](../../../tests/Jroc.Test262.Tests/language/expressions/JavaScript/tco-pos.js)<br>[`tco-member-args.js`](../../../tests/Jroc.Test262.Tests/language/expressions/call/JavaScript/tco-member-args.js)<br>[`tco-pos-null.js`](../../../tests/Jroc.Test262.Tests/language/expressions/coalesce/JavaScript/tco-pos-null.js)<br>[`tco-pos-undefined.js`](../../../tests/Jroc.Test262.Tests/language/expressions/coalesce/JavaScript/tco-pos-undefined.js)<br>[`tco-final.js`](../../../tests/Jroc.Test262.Tests/language/expressions/comma/JavaScript/tco-final.js)<br>[`tco-stmt.js`](../../../tests/Jroc.Test262.Tests/language/statements/block/JavaScript/tco-stmt.js)<br>[`tco-stmt-list.js`](../../../tests/Jroc.Test262.Tests/language/statements/block/JavaScript/tco-stmt-list.js)<br>[`tco-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/do-while/JavaScript/tco-body.js)<br>[`tco-const-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-const-body.js)<br>[`tco-let-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-let-body.js)<br>[`tco-lhs-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-lhs-body.js)<br>[`tco-var-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/for/JavaScript/tco-var-body.js)<br>[`tco-else-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/tco-else-body.js)<br>[`tco-if-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/tco-if-body.js)<br>[`tco.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/tco.js)<br>[`tco.js`](../../../tests/Jroc.Test262.Tests/language/statements/return/JavaScript/tco.js)<br>[`tco-case-body-dflt.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-case-body-dflt.js)<br>[`tco-case-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-case-body.js)<br>[`tco-dftl-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/switch/JavaScript/tco-dftl-body.js)<br>[`tco-body.js`](../../../tests/Jroc.Test262.Tests/language/statements/while/JavaScript/tco-body.js) | `test/language/expressions/tco-pos.js`<br>`test/language/expressions/call/tco-member-args.js`<br>`test/language/expressions/coalesce/tco-pos-null.js`<br>`test/language/expressions/coalesce/tco-pos-undefined.js`<br>`test/language/expressions/comma/tco-final.js`<br>`test/language/statements/block/tco-stmt.js`<br>`test/language/statements/block/tco-stmt-list.js`<br>`test/language/statements/do-while/tco-body.js`<br>`test/language/statements/for/tco-const-body.js`<br>`test/language/statements/for/tco-let-body.js`<br>`test/language/statements/for/tco-lhs-body.js`<br>`test/language/statements/for/tco-var-body.js`<br>`test/language/statements/if/tco-else-body.js`<br>`test/language/statements/if/tco-if-body.js`<br>`test/language/statements/labeled/tco.js`<br>`test/language/statements/return/tco.js`<br>`test/language/statements/switch/tco-case-body-dflt.js`<br>`test/language/statements/switch/tco-case-body.js`<br>`test/language/statements/switch/tco-dftl-body.js`<br>`test/language/statements/while/tco-body.js` | Eligible direct calls use tail. call ret rather than growing the .NET stack. Tail-position recognition covers direct returns, conditional branches, logical AND/OR, and object-ABI nullish-coalescing right operands and final comma operands. The new comma and nullish cases pass at the pinned tcoHelper.js depth of 100,000. Calls requiring argument objects, rest/spread handling, or function-object invocation are not eligible; constructors, async/generator functions, and protected returns retain their existing lowering. |

