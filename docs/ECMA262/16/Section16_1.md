<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 16.1: Scripts

[Back to Section16](Section16.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-08T21:54:24Z

JROC parses top-level source with `ParseScript`, validates it under a strict-mode policy, and executes it through a CommonJS module wrapper rather than the ECMA-262 Script Record / global-environment pipeline. Entry scripts run successfully, but top-level behavior is intentionally closer to Node/CommonJS hosting than spec script evaluation.

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
| Script-code parsing and early errors | Supported with Limitations | `tests/Jroc.Test262.Tests/language/global-code/LexicalSourceConformanceBatchTests.cs` |  | Twenty-five pinned Test262 global-code fixtures are natively verified for script syntax, early errors, strict-mode declarations, and permitted top-level forms. Runtime execution remains CommonJS-wrapped rather than full Script Record evaluation. |
| top-level scripts compiled as CommonJS-wrapped entry modules | Supported with Limitations |  |  | Compiled entry files execute through `ModuleMainDelegate(exports, require, module, __filename, __dirname)` and CommonJS module scope. Top-level declarations therefore do not go through spec Script Records or a true global environment record. |

### 16.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-scriptisstrict))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| strict-mode directive prologue detection and enforcement policy | Supported with Limitations |  |  | JROC detects a leading `"use strict"` directive and, by default, requires it for successful compilation. `CompilerOptions.StrictMode` can downgrade missing strict mode to a warning or ignore it, but the compiler/runtime are designed around strict-mode semantics. |

