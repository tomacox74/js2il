<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.4: Function Name Inference

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.4 | Function Name Inference | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-function-name-inference) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 8.4.1 | Static Semantics: HasName | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasname) |
| 8.4.2 | Static Semantics: IsFunctionDefinition | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isfunctiondefinition) |
| 8.4.3 | Static Semantics: IsAnonymousFunctionDefinition ( expr ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isanonymousfunctiondefinition) |
| 8.4.4 | Static Semantics: IsIdentifierRef | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isidentifierref) |
| 8.4.5 | Runtime Semantics: NamedEvaluation | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-namedevaluation) |

## Support

Feature-level support tracking with test script references.

### 8.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasname))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function-expression naming for internal scope/binding identity | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`Function_IIFE_Recursive.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IIFE_Recursive.js) | Named function expressions create internal bindings and anonymous function expressions receive deterministic internal scope names. |

### 8.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-isanonymousfunctiondefinition))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Anonymous function-expression detection in scope construction | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs` | Anonymous function expressions are distinguished for compiler-internal naming, but observable Function.name semantics are not fully implemented. |

### 8.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-namedevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| NamedEvaluation / SetFunctionName observable semantics | Not Yet Supported |  | JS2IL does not currently implement spec-complete runtime name inference/exposure (for example full Function.prototype.name behavior for anonymous function assignment targets). |

