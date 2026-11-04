<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 12.2: White Space

[Back to Section12](Section12.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-11-04T05:36:48Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 12.2 | White Space | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-white-space) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 12.2 ([tc39.es](https://tc39.es/ecma262/#sec-white-space))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| White-space lexical syntax | Supported with Limitations | `tests/Jroc.Test262.Tests/language/white-space/LexicalSourceConformanceBatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/white-space/RemainingPinnedExecutionTests.cs` |  | All 67 pinned white-space cases have outcomes: 51 verified passing and 16 explicit eval exclusions. Every required native variant of the supported cases passes, including six syntax negatives. Coverage includes separator placement, comments, strings, and legacy whitespace code points; eval-dependent cases retain their original assertions and remain excluded. |

