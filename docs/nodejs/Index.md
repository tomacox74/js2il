<!-- AUTO-GENERATED: generateNodeIndex.js -->

# Node.js Support Coverage

**Target Node.js Version:** `22.x LTS`

**Generated:** `2026-04-07T12:40:52Z`

## Summary

- **Modules:** 19
- **Globals:** 14
  - Supported: 14
  - Partial: 17

## Modules

| Module | Status | Documentation |
| --- | --- | --- |
| [child_process](child_process.md) | partial | [Node.js](https://nodejs.org/api/child_process.html) |
| [crypto](crypto.md) | partial | [Node.js](https://nodejs.org/api/crypto.html) |
| [events](events.md) | completed | [Node.js](https://nodejs.org/api/events.html) |
| [fs](fs.md) | partial | [Node.js](https://nodejs.org/api/fs.html) |
| [fs/promises](fs_promises.md) | partial | [Node.js](https://nodejs.org/api/fs.html#fspromisesapi) |
| [http](http.md) | partial | [Node.js](https://nodejs.org/api/http.html) |
| [https](https.md) | partial | [Node.js](https://nodejs.org/api/https.html) |
| [net](net.md) | partial | [Node.js](https://nodejs.org/api/net.html) |
| [os](os.md) | partial | [Node.js](https://nodejs.org/api/os.html) |
| [path](path.md) | partial | [Node.js](https://nodejs.org/api/path.html) |
| [perf_hooks](perf_hooks.md) | partial | [Node.js](https://nodejs.org/api/perf_hooks.html) |
| [process](process.md) | completed | [Node.js](https://nodejs.org/api/process.html) |
| [querystring](querystring.md) | partial | [Node.js](https://nodejs.org/api/querystring.html) |
| [stream](stream.md) | partial | [Node.js](https://nodejs.org/api/stream.html) |
| [timers/promises](timers_promises.md) | partial | [Node.js](https://nodejs.org/api/timers.html#timers-promises-api) |
| [tls](tls.md) | partial | [Node.js](https://nodejs.org/api/tls.html) |
| [url](url.md) | partial | [Node.js](https://nodejs.org/api/url.html) |
| [util](util.md) | partial | [Node.js](https://nodejs.org/api/util.html) |
| [zlib](zlib.md) | partial | [Node.js](https://nodejs.org/api/zlib.html) |

## Globals

| Global | Status | Documentation |
| --- | --- | --- |
| [__dirname](__dirname.md) | supported | [Node.js](https://nodejs.org/api/modules.html#dirname) |
| [__filename](__filename.md) | supported | [Node.js](https://nodejs.org/api/modules.html#filename) |
| [Buffer](Buffer.md) | supported | [Node.js](https://nodejs.org/api/buffer.html#class-buffer) |
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

- Buffer core APIs for binary workflows are implemented, including from/isBuffer/alloc/allocUnsafe/byteLength/concat/compare, slice/subarray/copy/write/fill/equals/indexOf/lastIndexOf/includes, array-like indexing, and binary read/write methods for Int8/16/32, UInt8/16/32, FloatLE/BE, and DoubleLE/BE with utf8/hex/base64 encodings.
- crypto currently implements a focused practical slice only: createHash/createHmac (md5/sha1/sha256/sha384/sha512), randomBytes, getRandomValues for Buffer/Uint8Array/Int32Array, and webcrypto.subtle digest (SHA-1/SHA-256/SHA-384/SHA-512) plus raw HMAC import/sign/verify. Callback-style APIs, pbkdf2Sync, ciphers, asymmetric keys, key generation/export, X.509/TLS crypto, and the broader Web Crypto matrix are not implemented.
- events module implements full core EventEmitter listener lifecycle APIs, events.errorMonitor behavior, and async helper APIs (events.on/events.once); advanced features such as captureRejections and newListener/removeListener event semantics are not yet implemented.
- Networking is currently a loopback-first IPv4 baseline: node:net supports createServer/connect/socket basics, Buffer-based socket reads by default, inherited pause()/resume()/utf8 setEncoding(), idle timeouts via setTimeout(), keepAlive enable/disable via setKeepAlive(), a compatibility no-op setNoDelay(), and allowHalfOpen delayed-response control for accepted server sockets in the supported local TCP slice. node:http now supports streamed Buffer-based IncomingMessage bodies, automatic chunked request/response framing when content-length is absent, forwarded server connection events, and sequential keep-alive reuse through http.Agent({ keepAlive: true }) for supported HTTP/1.1 flows. node:https and node:tls now add PEM-backed TLS loopback/local baselines via tls.createSecureContext(...), tls.createServer(...), tls.connect(...), https.createServer(...), https.request(...), and https.get(...) with rejectUnauthorized: false support for local self-signed scenarios. keepAlive initialDelay, HTTP pipelining, CONNECT tunneling, protocol upgrades, https.Agent pooling, custom CA trust, client certificates, ALPN, and broader TLS/OpenSSL option parity are still not implemented.
- CommonJS globals (__dirname/__filename) are supported; require() supports compiled local modules, implemented core modules, and compile-time node_modules discovery across .js/.mjs/.cjs files, package.json type=module entry graphs, package exports/imports condition selection for import/require/node/default, and package.json imports aliases that target either package-local ./... paths or bare package specifiers in the supported deterministic slice. Static import/export declarations and literal import()/require() package requests are resolved at compile time so import and require can target different package entries in one graph, and import.meta.url is available for compiled modules as a deterministic file:// URL. Runtime probing, custom loaders/hooks, nested package-imports aliases, and broader Node loader semantics beyond the documented slice remain unsupported.
- The fs/fs/promises baseline now includes whole-file helpers, FileHandle open/read/write/close, createReadStream/createWriteStream, and appendFile/rename/unlink with the documented r/r+/w/w+/a/a+ flag subset. Raw numeric-fd workflows, advanced metadata/permission parity, file watching, and broader stream option coverage remain out of scope.
- Only a small subset of Node is implemented to support tests; many APIs are unimplemented.
