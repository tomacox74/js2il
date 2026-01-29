<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.5: Error Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.5 | Error Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-error-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.5.1 | The Error Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-error-constructor) |
| 20.5.1.1 | Error ( message [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-error-message) |
| 20.5.2 | Properties of the Error Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-error-constructor) |
| 20.5.2.1 | Error.isError ( arg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-error.iserror) |
| 20.5.2.2 | Error.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-error.prototype) |
| 20.5.3 | Properties of the Error Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-error-prototype-object) |
| 20.5.3.1 | Error.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-error.prototype.constructor) |
| 20.5.3.2 | Error.prototype.message | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-error.prototype.message) |
| 20.5.3.3 | Error.prototype.name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-error.prototype.name) |
| 20.5.3.4 | Error.prototype.toString ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-error.prototype.tostring) |
| 20.5.4 | Properties of Error Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-error-instances) |
| 20.5.5 | Native Error Types Used in This Standard | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard) |
| 20.5.5.1 | EvalError | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard-evalerror) |
| 20.5.5.2 | RangeError | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard-rangeerror) |
| 20.5.5.3 | ReferenceError | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard-referenceerror) |
| 20.5.5.4 | SyntaxError | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard-syntaxerror) |
| 20.5.5.5 | TypeError | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard-typeerror) |
| 20.5.5.6 | URIError | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-native-error-types-used-in-this-standard-urierror) |
| 20.5.6 | NativeError Object Structure | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror-object-structure) |
| 20.5.6.1 | The NativeError Constructors | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror-constructors) |
| 20.5.6.1.1 | NativeError ( message [ , options ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror) |
| 20.5.6.2 | Properties of the NativeError Constructors | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-nativeerror-constructors) |
| 20.5.6.2.1 | NativeError .prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror.prototype) |
| 20.5.6.3 | Properties of the NativeError Prototype Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-nativeerror-prototype-objects) |
| 20.5.6.3.1 | NativeError .prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror.prototype.constructor) |
| 20.5.6.3.2 | NativeError .prototype.message | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror.prototype.message) |
| 20.5.6.3.3 | NativeError .prototype.name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-nativeerror.prototype.name) |
| 20.5.6.4 | Properties of NativeError Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-nativeerror-instances) |
| 20.5.7 | AggregateError Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error-objects) |
| 20.5.7.1 | The AggregateError Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error-constructor) |
| 20.5.7.1.1 | AggregateError ( errors , message [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error) |
| 20.5.7.2 | Properties of the AggregateError Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-aggregate-error-constructors) |
| 20.5.7.2.1 | AggregateError.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error.prototype) |
| 20.5.7.3 | Properties of the AggregateError Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-aggregate-error-prototype-objects) |
| 20.5.7.3.1 | AggregateError.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error.prototype.constructor) |
| 20.5.7.3.2 | AggregateError.prototype.message | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error.prototype.message) |
| 20.5.7.3.3 | AggregateError.prototype.name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-aggregate-error.prototype.name) |
| 20.5.7.4 | Properties of AggregateError Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-aggregate-error-instances) |
| 20.5.8 | Abstract Operations for Error Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-error-objects) |
| 20.5.8.1 | InstallErrorCause ( O , options ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-installerrorcause) |

## Support

Feature-level support tracking with test script references.

### 20.5.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-error-message))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Error(message) (callable creates instance) | Supported with Limitations | [`IntrinsicCallables_Error_Callable_CreatesInstances.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Error_Callable_CreatesInstances.js) | Compiler lowers built-in error callables (Error/TypeError/...) to construction of JavaScriptRuntime error types. Currently supports 0 or 1 argument; the optional 'options' parameter is not supported. |
| new Error(message) and new NativeError(message) | Supported with Limitations | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js) | Constructs JavaScriptRuntime.Error (and derived types) with message stringification via DotNet2JSConversions.ToString. The runtime does not currently model spec prototype objects; behavior is closer to .NET exceptions with JS-like surface properties. |

### 20.5.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-error-instances))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Error instance properties: name, message, stack | Supported with Limitations | [`TryCatch_CallMember_MissingMethod_IsTypeError.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_CallMember_MissingMethod_IsTypeError.js)<br>[`Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage.js) | name/message are exposed as instance properties on JavaScriptRuntime.Error. stack is backed by the .NET stack trace (or a captured construction-time stack if not thrown yet). |

### 20.5.7.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-aggregate-error))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| AggregateError constructors and .errors | Supported with Limitations | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js)<br>[`Promise_Any_AllRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Any_AllRejected.js) | JavaScriptRuntime.AggregateError stores errors in a JavaScriptRuntime.Array. Signature differs from spec in some cases (e.g., the test uses new AggregateError("agg") which maps to a message-only overload). 'options' / 'cause' are not supported. |

