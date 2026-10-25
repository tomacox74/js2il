<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.2: PrivateEnvironment Records

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:21Z

Class private fields, methods and accessors use lexically resolved compiler metadata and runtime receiver brands. Each class evaluation creates a distinct private-name identity. General eval-based private-name resolution remains unsupported.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.2 | PrivateEnvironment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateenvironment-records) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.2.1 | PrivateEnvironment Record Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateenvironment-record-operations) |
| 9.2.1.1 | NewPrivateEnvironment ( outerPrivateEnv ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newprivateenvironment) |
| 9.2.1.2 | ResolvePrivateIdentifier ( privateEnv , identifier ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-resolve-private-identifier) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-privateenvironment-record-operations))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| PrivateEnvironment record operations for class private fields, methods and accessors | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_ClassPrivateProperty_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateProperty_HelperMethod_Log.js)<br>[`Classes_ClassPrivateMethodAndAccessor_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateMethodAndAccessor_Log.js)<br>[`Classes_ClassPrivateAccessor_EdgeCases_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateAccessor_EdgeCases_Log.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js) |  | Private fields, methods and getter/setter accessors are lowered through the generated class private-name surface and enforce receiver brands. Getter-only and setter-only accessors reject invalid writes and reads respectively. This does not provide general eval access to a private environment. |

### 9.2.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-resolve-private-identifier))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ResolvePrivateIdentifier in supported class forms | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_PrivateNames_NestedFunctionDistinctClassEvaluations.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_PrivateNames_NestedFunctionDistinctClassEvaluations.js)<br>[`Classes_ClassPrivateAccessor_ClassExpression_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateAccessor_ClassExpression_Log.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js)<br>`tests/Jroc.Tests/ValidatorTests.cs` |  | Compiler lexical resolution handles private fields, methods and accessors, including nested functions and class expressions. Separately evaluated classes retain distinct private brands even when their source and public prototype chains match. General eval-based private identifier resolution remains unsupported. |

