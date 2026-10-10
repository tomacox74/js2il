<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 16.1: Scripts

[Back to Section16](Section16.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-10T03:30:17Z

JROC retains Node/CommonJS wrapper behavior by default. Explicit Script-goal compilation selects standard script syntax and realm-wide global declaration semantics, including separate lexical bindings. The native Test262 harness selects this goal for non-module fixtures. Full Script Record completion-value exposure and JavaScript eval remain unsupported.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 16.1 | Scripts | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-scripts) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 16.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-scripts-static-semantics-early-errors) |
| 16.1.2 | Static Semantics: ScriptIsStrict | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-scriptisstrict) |
| 16.1.3 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-script-semantics-runtime-semantics-evaluation) |
| 16.1.4 | Script Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-script-records) |
| 16.1.5 | ParseScript ( sourceText , realm , hostDefined ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parse-script) |
| 16.1.6 | ScriptEvaluation ( scriptRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-scriptevaluation) |
| 16.1.7 | GlobalDeclarationInstantiation ( script , env ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-globaldeclarationinstantiation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 16.1 ([tc39.es](https://tc39.es/ecma262/#sec-scripts))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Script-code parsing and early errors | Supported with Limitations | `tests/Jroc.Test262.Tests/language/global-code/LexicalSourceConformanceBatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/global-code/GlobalCodeCompletionExecutionTests.cs` |  | Forty-one pinned global-code cases are verified passing and one is explicitly excluded for eval. Script-goal parsing rejects top-level return and export declarations with SyntaxError. |
| top-level scripts compiled as CommonJS-wrapped entry modules | Supported with Limitations |  |  | The default entry mode executes through `ModuleMainDelegate(exports, require, module, __filename, __dirname)` and CommonJS module scope. Explicit ParseAsScript compilation instead gives source declarations realm-wide global binding semantics while retaining the compiled hosting ABI. |

### 16.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-scriptisstrict))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| strict-mode directive prologue detection and enforcement policy | Supported with Limitations |  |  | JROC detects a leading `"use strict"` directive and, by default, requires it for successful compilation. `CompilerOptions.StrictMode` can downgrade missing strict mode to a warning or ignore it, but the compiler/runtime are designed around strict-mode semantics. |

### 16.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-globaldeclarationinstantiation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| realm-owned global declaration instantiation across compiled scripts | Supported with Limitations |  | `test/language/global-code/decl-func.js`<br>`test/language/global-code/decl-lex-restricted-global.js`<br>`test/language/global-code/script-decl-func.js`<br>`test/language/global-code/script-decl-lex.js`<br>`test/language/global-code/script-decl-var-collision.js`<br>`test/language/global-code/script-decl-var-err.js` | Global var/function bindings preserve property attributes; lexical/class bindings remain separate from global-object properties with TDZ and const enforcement. Redeclaration, restricted-property, and extensibility checks run before any body statements or new bindings are installed. The native $262.evalScript host compiles scripts in the current realm; it is not language eval and currently does not expose script completion values. |

