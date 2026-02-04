<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 11.2: Types of Source Code

[Back to Section11](Section11.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 11.2 | Types of Source Code | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-types-of-source-code) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 11.2.1 | Directive Prologues and the Use Strict Directive | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-directive-prologues-and-the-use-strict-directive) |
| 11.2.2 | Strict Mode Code | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-strict-mode-code) |
| 11.2.2.1 | Static Semantics: IsStrict ( node ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isstrict) |
| 11.2.3 | Non-ECMAScript Functions | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-non-ecmascript-functions) |

## Support

Feature-level support tracking with test script references.

### 11.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-directive-prologues-and-the-use-strict-directive))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Directive prologues; configurable enforcement of "use strict" | Supported with Limitations |  | JS2IL recognizes directive prologues and (by default) requires a top-level "use strict" directive. For third-party CommonJS modules that omit the directive, the compiler can downgrade the missing prologue to a warning via --strictMode=Warn (or suppress via --strictMode=Ignore). JS2IL still compiles using strict-mode semantics; this option only changes reporting severity for the missing directive prologue. |

