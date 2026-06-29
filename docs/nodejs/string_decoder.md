# Module: string_decoder

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/string_decoder.html) |

## Implementation

- `src/JavaScriptRuntime/Node/StringDecoderModule.cs`

## Notes

Supports utf8 and utf-8 only. Both require('string_decoder') and require('node:string_decoder') resolve to the module.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| StringDecoder(encoding) | function | partial | [docs](https://nodejs.org/api/string_decoder.html#new-stringdecoderencoding) |
| write(buffer) | function | partial | [docs](https://nodejs.org/api/string_decoder.html#stringdecoderwritebuffer) |
| end([buffer]) | function | partial | [docs](https://nodejs.org/api/string_decoder.html#stringdecoderendbuffer) |

## API Details

### StringDecoder(encoding)

Constructs a decoder that exposes write(buffer) and end([buffer]) for buffered utf8 decoding.

**Tests:**
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_Utf8_Basic` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_NodePrefix` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_MemoryStreamStyle` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_Utf8_Basic` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_NodePrefix` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_MemoryStreamStyle` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)

### write(buffer)

Buffers incomplete multibyte sequences and returns decoded text.

**Tests:**
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_Utf8_Basic` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_NodePrefix` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_MemoryStreamStyle` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_Utf8_Basic` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_NodePrefix` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_MemoryStreamStyle` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)

### end([buffer])

Flushes any buffered partial utf8 sequence and accepts an optional final buffer.

**Tests:**
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_Utf8_Basic` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_NodePrefix` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.ExecutionTests.Require_StringDecoder_MemoryStreamStyle` (`tests/Jroc.Tests/Node/StringDecoder/ExecutionTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_Utf8_Basic` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_NodePrefix` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
- `Jroc.Tests.Node.StringDecoder.GeneratorTests.Require_StringDecoder_MemoryStreamStyle` (`tests/Jroc.Tests/Node/StringDecoder/GeneratorTests.cs`)
