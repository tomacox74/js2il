<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.1: Identifiers

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.1 | Identifiers | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-identifiers) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.1.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-identifiers-static-semantics-early-errors) |
| 13.1.2 | Static Semantics: StringValue | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-stringvalue) |
| 13.1.3 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-identifiers-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 13.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-identifiers-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Numeric literals (integer and decimal) | Supported | [`BinaryOperator_AddNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js)<br>[`BinaryOperator_MulNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_MulNumberNumber.js) | Numbers are represented as double and used pervasively across arithmetic, comparison, and control-flow tests. |

### 13.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-stringvalue))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String literals (single/double quotes; escapes) | Supported | [`String_StartsWith_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_StartsWith_Basic.js)<br>[`String_Replace_Regex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js) | Backed by .NET System.String; values are boxed/unboxed where needed in member calls and concatenation. |

### 13.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-identifiers-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Boolean literals (true/false) | Supported | [`UnaryOperator_Typeof.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js)<br>[`JSON_Parse_SimpleObject.js`](../../../Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js) | Emits proper IL for true/false and boxes when needed in arrays/log calls. See generator snapshot: Js2IL.Tests/Literals/GeneratorTests.BooleanLiteral.verified.txt. |

### 13.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-template-literals))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Template literals (basic interpolation) | Supported | [`String_TemplateLiteral_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_TemplateLiteral_Basic.js) | Concatenates quasis and expressions via runtime Operators.Add with JS string/number coercion. Tagged templates are not yet supported. |

### 13.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-null-literals))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| null literal | Supported | [`Literals_NullAndUndefined.js`](../../../Js2IL.Tests/Literals/JavaScript/Literals_NullAndUndefined.js)<br>[`JSON_Parse_SimpleObject.js`](../../../Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js) | null emission validated in literals and variable tests; see execution snapshot Js2IL.Tests/Literals/ExecutionTests.Literals_NullAndUndefined.verified.txt. |

### 13.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-undefined))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| undefined identifier usage | Supported | [`Literals_NullAndUndefined.js`](../../../Js2IL.Tests/Literals/JavaScript/Literals_NullAndUndefined.js) | Handled as the ECMAScript undefined value and participates in JS truthiness; see execution snapshot Js2IL.Tests/Literals/ExecutionTests.Literals_NullAndUndefined.verified.txt. |

### 13.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-this-keyword))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| this binding in non-arrow functions | Supported | [`Function_ObjectLiteralMethod_ThisBinding.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js) | Member calls bind the receiver as runtime this. |

