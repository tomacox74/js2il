<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 12.7: Names and Keywords

[Back to Section12](Section12.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-17T05:03:58Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 12.7 | Names and Keywords | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-names-and-keywords) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 12.7.1 | Identifier Names | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifier-names) |
| 12.7.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifier-names-static-semantics-early-errors) |
| 12.7.1.2 | Static Semantics: IdentifierCodePoints | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifiercodepoints) |
| 12.7.1.3 | Static Semantics: IdentifierCodePoint | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifiercodepoint) |
| 12.7.2 | Keywords and Reserved Words | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-keywords-and-reserved-words) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 12.7 ([tc39.es](https://tc39.es/ecma262/#sec-names-and-keywords))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Identifier names, keywords, and reserved-word syntax | Supported with Limitations | `tests/Jroc.Test262.Tests/language/identifiers/LexicalSourceConformanceBatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/identifiers/IdentifiersImportCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/keywords/LexicalSourceConformanceBatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/future-reserved-words/LexicalSourceConformanceBatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/reserved-words/LexicalSourceConformanceBatchTests.cs` |  | Pinned native coverage includes Unicode identifier names, keywords, future reserved words, and reserved-word property-name syntax. All 268 standalone language/identifiers cases now have verified results, including 116 additional syntax negatives covering reserved bindings, escaped keywords, and invalid identifier code points. Every required strict/non-strict variant is checked; no case in this area requires an eval exclusion. |

