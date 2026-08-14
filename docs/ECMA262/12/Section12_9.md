<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 12.9: Literals

[Back to Section12](Section12.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-14T06:17:38Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 12.9 | Literals | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-lexical-grammar-literals) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 12.9.1 | Null Literals | Supported | [tc39.es](https://tc39.es/ecma262/#sec-null-literals) |
| 12.9.2 | Boolean Literals | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean-literals) |
| 12.9.3 | Numeric Literals | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-literals-numeric-literals) |
| 12.9.3.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-literals-early-errors) |
| 12.9.3.2 | Static Semantics: MV | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-mv) |
| 12.9.3.3 | Static Semantics: NumericValue | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numericvalue) |
| 12.9.4 | String Literals | Supported | [tc39.es](https://tc39.es/ecma262/#sec-literals-string-literals) |
| 12.9.4.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string-literals-early-errors) |
| 12.9.4.2 | Static Semantics: SV | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-sv) |
| 12.9.4.3 | Static Semantics: MV | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-string-literals-static-semantics-mv) |
| 12.9.5 | Regular Expression Literals | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-literals-regular-expression-literals) |
| 12.9.5.1 | Static Semantics: BodyText | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-bodytext) |
| 12.9.5.2 | Static Semantics: FlagText | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-flagtext) |
| 12.9.6 | Template Literal Lexical Components | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-template-literal-lexical-components) |
| 12.9.6.1 | Static Semantics: TV | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-tv) |
| 12.9.6.2 | Static Semantics: TRV | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-trv) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 12.9.3 ([tc39.es](https://tc39.es/ecma262/#sec-literals-numeric-literals))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Numeric literal parsing and numeric values | Supported with Limitations | `tests/Jroc.Test262.Tests/language/literals/numeric/ExecutionTests.cs` |  | Fifty upstream test262 numeric-literal cases cover decimal integer, decimal-point, and exponent forms and their resulting numeric values. This evidence does not yet comprehensively cover every modern numeric-literal extension. |

