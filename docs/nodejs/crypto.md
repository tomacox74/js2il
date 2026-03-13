# Module: crypto

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/crypto.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Crypto.cs`

## Notes

Provides a focused crypto baseline for common hashing and secure-random workflows. Only synchronous createHash/randomBytes plus crypto.getRandomValues and crypto.webcrypto.getRandomValues are implemented; HMAC, ciphers, keys, sign/verify, and webcrypto.subtle are not yet available.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createHash(algorithm) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptocreatehashalgorithm-options) |
| randomBytes(size) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptorandombytessize-callback) |
| getRandomValues(typedArray) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptogetrandomvaluestypedarray) |
| webcrypto.getRandomValues(typedArray) | method | supported | [docs](https://nodejs.org/api/webcrypto.html#cryptogetrandomvaluestypedarray) |

## API Details

### createHash(algorithm)

Supports md5, sha1, sha256, sha384, and sha512. Returned Hash objects support update(data[, inputEncoding]) and digest([outputEncoding]).

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_CreateHash_And_RandomBytes` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_CreateHash_And_RandomBytes` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)

### randomBytes(size)

Returns a Buffer of the requested size using the platform cryptographic random source. Callback-style randomBytes is not implemented.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_CreateHash_And_RandomBytes` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_CreateHash_And_RandomBytes` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)

### getRandomValues(typedArray)

Aliased to webcrypto.getRandomValues for Buffer, Uint8Array, and Int32Array.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_GetRandomValues` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_GetRandomValues` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)

### webcrypto.getRandomValues(typedArray)

Supports Buffer, Uint8Array, and Int32Array. Other Web Crypto APIs are not implemented yet.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_GetRandomValues` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_GetRandomValues` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)
