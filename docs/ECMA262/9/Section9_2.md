<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.2: PrivateEnvironment Records

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

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

Feature-level support tracking with test script references.

### 9.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-privateenvironment-record-operations))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| PrivateEnvironment record operations for class private fields | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_ClassPrivateProperty_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateProperty_HelperMethod_Log.js) | Private field declarations/access use compiler-managed metadata and runtime helpers; private methods/accessors are still incomplete. |

### 9.2.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-resolve-private-identifier))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ResolvePrivateIdentifier in supported class forms | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>`Js2IL.Tests/ValidatorTests.cs` | Private identifier resolution is supported for private fields/properties; validator rejects unsupported private methods/accessors. |

