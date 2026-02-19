# Module: util

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/util.html) |

## Implementation

- `JavaScriptRuntime/Node/Util.cs`

## Notes

Provides essential utility functions for promisifying callbacks, prototype inheritance, type checking, and object inspection.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| promisify(callback) | function | supported | [docs](https://nodejs.org/api/util.html#utilpromisifyoriginal) |
| inherits(constructor, superConstructor) | function | supported | [docs](https://nodejs.org/api/util.html#utilinheritsconstructor-superconstructor) |
| types | property | partial | [docs](https://nodejs.org/api/util.html#utiltypes) |
| inspect(value[, options]) | function | partial | [docs](https://nodejs.org/api/util.html#utilinspectobject-options) |

## API Details

### promisify(callback)

Converts error-first callback functions to Promise-returning functions. Follows Node.js error-first convention where the first argument is an error or null, and the second is the result.

**Tests:**
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Promisify_Basic` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Promisify_ErrorHandling` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Promisify_Basic` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Promisify_ErrorHandling` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)

### inherits(constructor, superConstructor)

Sets up prototype chain inheritance between constructor functions. This is a legacy API; modern code should use ES6 classes instead. Sets the super_ property on the constructor.

**Tests:**
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Inherits_Basic` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Inherits_Basic` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)

### types

Provides type-checking functions. Supported checks: isArray, isDate, isError, isFunction, isPromise, isRegExp, isString, isNumber, isBoolean, isUndefined, isNull, isObject, isBigInt, isSymbol, isAsyncFunction. Additional Node.js type checks (typed arrays, shared buffers, etc.) are not yet implemented.

**Tests:**
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Types_IsPromise` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Types_IsArray` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Types_IsFunction` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Types_IsPromise` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Types_IsArray` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Types_IsFunction` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)

### inspect(value[, options])

Provides basic object inspection for debugging. Supports depth limiting and handles circular references. Options supported: depth, showHidden, colors (parsed but colors not applied to output). Does not implement full Node.js inspect formatting or custom inspect symbols.

**Tests:**
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Inspect_Basic` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.ExecutionTests.Require_Util_Inspect_Object` (`Js2IL.Tests/Node/Util/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Inspect_Basic` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Util.GeneratorTests.Require_Util_Inspect_Object` (`Js2IL.Tests/Node/Util/GeneratorTests.cs`)
