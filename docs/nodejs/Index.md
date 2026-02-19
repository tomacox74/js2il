<!-- AUTO-GENERATED: generateNodeIndex.js -->

# Node.js Support Coverage

**Target Node.js Version:** `22.x LTS`

**Generated:** `2026-02-19T01:37:48Z`

## Summary

- **Modules:** 8
- **Globals:** 14
  - Supported: 13
  - Partial: 8

## Modules

| Module | Status | Documentation |
| --- | --- | --- |
| [child_process](child_process.md) | partial | [Node.js](https://nodejs.org/api/child_process.html) |
| [events](events.md) | completed | [Node.js](https://nodejs.org/api/events.html) |
| [fs](fs.md) | partial | [Node.js](https://nodejs.org/api/fs.html) |
| [fs/promises](fs_promises.md) | partial | [Node.js](https://nodejs.org/api/fs.html#fspromisesapi) |
| [os](os.md) | partial | [Node.js](https://nodejs.org/api/os.html) |
| [path](path.md) | partial | [Node.js](https://nodejs.org/api/path.html) |
| [perf_hooks](perf_hooks.md) | partial | [Node.js](https://nodejs.org/api/perf_hooks.html) |
| [process](process.md) | partial | [Node.js](https://nodejs.org/api/process.html) |

## Globals

| Global | Status | Documentation |
| --- | --- | --- |
| [__dirname](__dirname.md) | supported | [Node.js](https://nodejs.org/api/modules.html#dirname) |
| [__filename](__filename.md) | supported | [Node.js](https://nodejs.org/api/modules.html#filename) |
| [Buffer](Buffer.md) | partial | [Node.js](https://nodejs.org/api/buffer.html#class-buffer) |
| [clearImmediate](clearImmediate.md) | supported | [Node.js](https://nodejs.org/api/timers.html#clearimmediateimmediate) |
| [clearInterval](clearInterval.md) | supported | [Node.js](https://nodejs.org/api/timers.html#clearintervaltimeout) |
| [clearTimeout](clearTimeout.md) | supported | [Node.js](https://nodejs.org/api/timers.html#cleartimeouttimeout) |
| [console.error](console_error.md) | supported | [Node.js](https://nodejs.org/api/console.html#consoleerrordata-args) |
| [console.log](console_log.md) | supported | [Node.js](https://nodejs.org/api/console.html#consolelogdata-args) |
| [console.warn](console_warn.md) | supported | [Node.js](https://nodejs.org/api/console.html#consolewarndata-args) |
| [Promise](Promise.md) | supported | [Node.js](https://nodejs.org/api/globals.html#promise) |
| [require(id)](require(id).md) | supported | [Node.js](https://nodejs.org/api/modules.html#requireid) |
| [setImmediate](setImmediate.md) | supported | [Node.js](https://nodejs.org/api/timers.html#setimmediatecallback-args) |
| [setInterval](setInterval.md) | supported | [Node.js](https://nodejs.org/api/timers.html#setintervalcallback-delay-args) |
| [setTimeout](setTimeout.md) | supported | [Node.js](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args) |

## Limitations

- Buffer support is partial (core APIs like from/isBuffer/alloc/byteLength/concat and utf8/hex/base64 string conversions are implemented; many advanced APIs remain unimplemented).
- events module support is partial with baseline EventEmitter listener lifecycle APIs; advanced emitter APIs are not implemented yet.
- CommonJS globals (__dirname/__filename) are supported; require() is partially supported for compiled local modules and implemented core modules; ESM import.meta.url is not.
- Only a small subset of Node is implemented to support tests; many APIs are unimplemented.
