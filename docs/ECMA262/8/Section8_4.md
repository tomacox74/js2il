<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.4: Function Name Inference

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T21:06:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.4 | Function Name Inference | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-function-name-inference) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 8.4.1 | Static Semantics: HasName | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasname) |
| 8.4.2 | Static Semantics: IsFunctionDefinition | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isfunctiondefinition) |
| 8.4.3 | Static Semantics: IsAnonymousFunctionDefinition ( expr ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isanonymousfunctiondefinition) |
| 8.4.4 | Static Semantics: IsIdentifierRef | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isidentifierref) |
| 8.4.5 | Runtime Semantics: NamedEvaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-namedevaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 8.4 ([tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-function-name-inference))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function name inference across declarations, anonymous initializers, object/class property definitions, and dynamic function creation | Supported with Limitations |  | `test/language/statements/const/fn-name-arrow.js`<br>`test/language/expressions/object/__proto__-fn-name.js`<br>`test/language/statements/function/name.js`<br>`test/built-ins/GeneratorFunction/instance-name.js`<br>`test/language/expressions/class/elements/class-name-static-initializer-expr.js` | JROC covers the observable name-inference behavior used by today's supported declarations, anonymous initializer bindings, object/class element definitions, and dynamic generator-function creation. Remaining limitations are in spec-edge name-prefix cases and unsupported runtime forms rather than in the common SetFunctionName / NamedEvaluation paths covered here. |

### 8.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasname))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function-expression naming for internal scope/binding identity | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Function_IIFE_Recursive.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_IIFE_Recursive.js) | `test/language/expressions/function/named-no-strict-reassign-fn-name-in-body.js`<br>`test/language/expressions/function/named-strict-error-reassign-fn-name-in-body.js` | Named function expressions create internal bindings and anonymous function expressions receive deterministic internal scope names. |

### 8.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isfunctiondefinition))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsFunctionDefinition classification for anonymous initializer name inference sites | Supported with Limitations |  | `test/language/statements/const/fn-name-arrow.js`<br>`test/language/statements/const/fn-name-fn.js`<br>`test/language/statements/const/fn-name-gen.js`<br>`test/language/statements/const/fn-name-class.js` | The compiler/runtime correctly recognize the covered function, arrow, generator, and class initializer forms that participate in name inference for bindings and property definitions. |

### 8.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-isanonymousfunctiondefinition))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Anonymous function-expression detection in scope construction | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs` | `test/language/expressions/arrow-function/dstr/ary-ptrn-elem-id-init-fn-name-arrow.js`<br>`test/language/expressions/function/dstr/ary-ptrn-elem-id-init-fn-name-fn.js` | Anonymous function expressions are distinguished for compiler-internal naming and for the covered observable name-inference sites used by destructuring defaults and property initialization. |

### 8.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isidentifierref))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsIdentifierRef participation in binding and destructuring name inference | Supported with Limitations |  | `test/language/expressions/function/dstr/obj-ptrn-id-init-fn-name-gen.js`<br>`test/language/expressions/arrow-function/dstr/ary-ptrn-elem-id-init-fn-name-cover.js` | The covered destructuring-default and binding-identifier paths infer names from identifier references correctly where current supported syntax depends on them. |

### 8.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-namedevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NamedEvaluation / SetFunctionName observable semantics | Supported with Limitations |  | `test/language/statements/const/fn-name-arrow.js`<br>`test/language/expressions/object/__proto__-fn-name.js`<br>`test/language/statements/function/name.js`<br>`test/built-ins/GeneratorFunction/instance-name.js`<br>`test/language/expressions/class/elements/class-name-static-initializer-anonymous.js` | JROC now implements the covered observable NamedEvaluation / SetFunctionName behavior for declarations, anonymous binding initializers, object/class property definitions, and dynamic generator-function creation. Some spec-edge prefixing and unsupported runtime forms still keep this below full spec-complete parity. |

