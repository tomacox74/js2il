<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.11: The with Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-14T20:01:50Z

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
| with statement | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/arrow-function/ArrowFunctionCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/compound-assignment/ExecutionTests.cs` |  | Non-strict with environments perform dynamic property lookup and honor Symbol.unscopables in active and captured environments. A blocked name falls through to the enclosing lexical or global binding; resolved assignment references are preserved through getter and right-hand-side side effects. Strict-mode with statements are parse errors. Direct eval remains unsupported. |

