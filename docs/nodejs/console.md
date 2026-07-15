# Module: console

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/console.html) |

## Implementation

- `src/JavaScriptRuntime/Node/ConsoleModule.cs`
- `src/JavaScriptRuntime/Console.cs`

## Notes

Supports require('console') and require('node:console'). The module exports a constructible Console class for routing output to JROC writable streams.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| Console(options) | property | partial | [docs](https://nodejs.org/api/console.html#new-consoleoptions) |
| console.log(...data) | function | supported | [docs](https://nodejs.org/api/console.html#consolelogdata) |
| console.warn(...data) | function | supported | [docs](https://nodejs.org/api/console.html#consolewarndata) |
| console.error(...data) | function | supported | [docs](https://nodejs.org/api/console.html#consoleerrordata) |
| console.table(tabularData) | function | partial | [docs](https://nodejs.org/api/console.html#consoletabletabulardata-properties) |

## API Details

### Console(options)

Supports object-form construction with stdout, optional stderr, and inspectOptions. stdout and stderr must be JROC Writable streams.

**Tests:**
- `Jroc.Tests.Node.Console.ExecutionTests.Console_Undici_Transform_Table` (`tests/Jroc.Tests/Node/Console/ExecutionTests.cs`)
- `Jroc.Tests.Node.Console.GeneratorTests.Console_Undici_Transform_Table` (`tests/Jroc.Tests/Node/Console/GeneratorTests.cs`)

### console.log(...data)

Writes formatted values followed by a newline to stdout.

**Tests:**
- `Jroc.Tests.ConsoleTests.Log_PrintsAllArgumentsWithSpaces` (`tests/Jroc.Tests/ConsoleTests.cs`)

### console.warn(...data)

Writes formatted values followed by a newline to stderr.

**Tests:**
- `Jroc.Tests.ConsoleTests.Warn_PrintsAllArgumentsWithSpaces_ToStdErr` (`tests/Jroc.Tests/ConsoleTests.cs`)

### console.error(...data)

Writes formatted values followed by a newline to stderr.

**Tests:**
- `Jroc.Tests.ConsoleTests.Error_PrintsAllArgumentsWithSpaces_ToStdErr` (`tests/Jroc.Tests/ConsoleTests.cs`)

### console.table(tabularData)

Formats arrays of object rows into a text table and writes the result to stdout. The optional properties argument and advanced Node table formatting are not yet supported.

**Tests:**
- `Jroc.Tests.Node.Console.ExecutionTests.Console_Undici_Transform_Table` (`tests/Jroc.Tests/Node/Console/ExecutionTests.cs`)
- `Jroc.Tests.Node.Console.GeneratorTests.Console_Undici_Transform_Table` (`tests/Jroc.Tests/Node/Console/GeneratorTests.cs`)
