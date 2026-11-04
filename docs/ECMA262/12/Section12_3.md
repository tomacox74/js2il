<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 12.3: Line Terminators

[Back to Section12](Section12.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-11-04T05:36:48Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 12.3 | Line Terminators | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-line-terminators) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 12.3 ([tc39.es](https://tc39.es/ecma262/#sec-line-terminators))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Line-terminator lexical syntax | Supported with Limitations | `tests/Jroc.Test262.Tests/language/line-terminators/LexicalSourceConformanceBatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/line-terminators/RemainingPinnedExecutionTests.cs` |  | All 41 pinned line-terminator cases have outcomes: 32 verified passing and nine explicit eval exclusions. Every required native variant of the supported cases passes, including 17 syntax negatives and runtime-negative comment cases. Coverage includes CR, LF, CRLF, line separator, and paragraph separator behavior. |

