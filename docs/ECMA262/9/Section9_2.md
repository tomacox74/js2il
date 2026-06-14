<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.2: PrivateEnvironment Records

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-18T20:54:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.2 | PrivateEnvironment Records | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-privateenvironment-records) |

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
| PrivateEnvironment record operations for class private fields | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_ClassPrivateProperty_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateProperty_HelperMethod_Log.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js) |  | Private field declarations/access use compiler-managed metadata and runtime helpers. Supported private methods use the generated class private-name surface and enforce receiver checks; private accessors remain incomplete. |

### 9.2.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-resolve-private-identifier))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ResolvePrivateIdentifier in supported class forms | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js)<br>`tests/Jroc.Tests/ValidatorTests.cs` |  | Private identifier resolution is supported for private fields/properties and supported private methods. Unsupported private accessor forms are still rejected. |

