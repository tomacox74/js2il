# Module: util

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/util.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Util.cs`

## Notes

Provides essential utility functions for promisifying callbacks, prototype inheritance, placeholder formatting, Node-style object inspection, and type checking for common runtime-backed binary/object shapes.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| promisify(callback) | function | supported | [docs](https://nodejs.org/api/util.html#utilpromisifyoriginal) |
| inherits(constructor, superConstructor) | function | supported | [docs](https://nodejs.org/api/util.html#utilinheritsconstructor-superconstructor) |
| format(format[, ...args]) | function | supported | [docs](https://nodejs.org/api/util.html#utilformatformat-args) |
| types | property | partial | [docs](https://nodejs.org/api/util.html#utiltypes) |
| inspect(value[, options]) | function | partial | [docs](https://nodejs.org/api/util.html#utilinspectobject-options) |

## API Details

### promisify(callback)

Converts error-first callback functions to Promise-returning functions. Follows Node.js error-first convention where the first argument is an error or null, and the second is the result.

**Tests:**
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Promisify_Basic` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Promisify_ErrorHandling` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Promisify_Basic` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Promisify_ErrorHandling` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)

### inherits(constructor, superConstructor)

Sets up prototype chain inheritance between constructor functions. This is a legacy API; modern code should use ES6 classes instead. Sets the super_ property on the constructor.

**Tests:**
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Inherits_Basic` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Inherits_Basic` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)

### format(format[, ...args])

Formats a string using placeholders like %s, %d/%i, %f, %j, %o/%O, and %% and appends any extra arguments separated by spaces.

**Tests:**
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Format_Basic` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Format_Basic` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)

### types

Provides type-checking functions. Supported checks: isArray, isDate, isError, isFunction, isPromise, isRegExp, isString, isNumber, isBoolean, isUndefined, isNull, isObject, isBigInt, isSymbol, isAsyncFunction, isMap, isSet, isProxy, isTypedArray, isAnyArrayBuffer, isArrayBuffer, isDataView, isUint8Array, isInt32Array, and isFloat64Array. Compatibility shims for unsupported runtime types such as SharedArrayBuffer and non-implemented typed-array flavors are also exposed and currently return false.

**Tests:**
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Types_IsPromise` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Types_IsArray` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Types_IsFunction` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Types_Expanded` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Types_TypedBinary` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Types_IsPromise` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Types_IsArray` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Types_IsFunction` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Types_Expanded` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Types_TypedBinary` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)

### inspect(value[, options])

Provides basic object inspection for debugging. Supports depth limiting, circular references, typed-array/ArrayBuffer/DataView formatting, and Node's custom inspect hook via util.inspect.custom (Symbol.for('nodejs.util.inspect.custom')). Options supported: depth, showHidden, colors (parsed but colors not applied to output).

**Tests:**
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Inspect_Basic` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Inspect_Object` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Inspect_Custom` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.ExecutionTests.Require_Util_Inspect_TypedBinary` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Inspect_Basic` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Inspect_Object` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Inspect_Custom` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_Util_Inspect_TypedBinary` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)
