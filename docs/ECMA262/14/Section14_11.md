<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.11: The with Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-21T20:52:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.11 | The with Statement | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-with-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.11.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-with-statement-static-semantics-early-errors) |
| 14.11.2 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-with-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.11 ([tc39.es](https://tc39.es/ecma262/#sec-with-statement))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| with statement | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/arrow-function/ArrowFunctionCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/compound-assignment/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/with/VariableWithCompletionExecutionTests.cs`<br>`tests/Jroc.Tests/HIRWithBindingTests.cs`<br>`tests/Jroc.Tests/SymbolTableBuilderTests.cs` | `test/language/statements/with/12.10-0-9.js`<br>`test/language/statements/with/12.10-2-1.js`<br>`test/language/statements/with/S12.10_A1.1_T1.js`<br>`test/language/statements/with/get-binding-value-call-with-proxy-env.js`<br>`test/language/statements/with/get-mutable-binding-binding-deleted-in-get-unscopables-strict-mode.js`<br>`test/language/statements/with/set-mutable-binding-idref-compound-assign-with-proxy-env.js` | All 181 applicable pinned with-statement cases have a documented outcome: 164 verified passing and 17 known unsupported eval cases. Non-strict with environments coerce their value to an object once, preserve captured outer variables, and dynamically shadow undefined, NaN, and Infinity in active and captured environments. Symbol.unscopables is checked during HasBinding; GetBindingValue and SetMutableBinding separately recheck property existence without repeating unscopables, preserving Proxy trap order, receiver identity, and strict-mode errors when a binding disappears. Sloppy var initializers retain sloppy write semantics. Blocked names fall through to the enclosing lexical or global binding, and resolved assignment references survive getter/right-hand-side side effects. Strict-mode with statements are parse errors. Direct and indirect eval remain unsupported. Every required native variant passed under one branch-local build; this is not central trusted acceptance. |
