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

Provides a focused practical crypto slice for hashing, secure random bytes, HMAC, and a minimal Web Crypto bridge. Supported Node algorithms are createHash/createHmac with md5, sha1, sha256, sha384, and sha512. Supported webcrypto.subtle operations are digest with SHA-1/SHA-256/SHA-384/SHA-512 plus importKey("raw", ...) / sign(...) / verify(...) for HMAC keys. Unsupported algorithms, formats, and key usages fail explicitly. Ciphers, pbkdf2Sync, asymmetric sign/verify, key generation/export, X.509/TLS crypto, and the broader Web Crypto matrix remain unimplemented.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createHash(algorithm) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptocreatehashalgorithm-options) |
| createHmac(algorithm, key) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptocreatehmacalgorithm-key-options) |
| randomBytes(size) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptorandombytessize-callback) |
| getRandomValues(typedArray) | function | supported | [docs](https://nodejs.org/api/crypto.html#cryptogetrandomvaluestypedarray) |
| webcrypto.getRandomValues(typedArray) | method | supported | [docs](https://nodejs.org/api/webcrypto.html#cryptogetrandomvaluestypedarray) |
| webcrypto.subtle.digest(algorithm, data) | method | supported | [docs](https://nodejs.org/api/webcrypto.html#subtledigestalgorithm-data) |
| webcrypto.subtle.importKey(format, keyData, algorithm, extractable, keyUsages) | method | supported | [docs](https://nodejs.org/api/webcrypto.html#subtleimportkeyformat-keydata-algorithm-extractable-keyusages) |
| webcrypto.subtle.sign(algorithm, key, data) | method | supported | [docs](https://nodejs.org/api/webcrypto.html#subtlesignalgorithm-key-data) |
| webcrypto.subtle.verify(algorithm, key, signature, data) | method | supported | [docs](https://nodejs.org/api/webcrypto.html#subtleverifyalgorithm-key-signature-data) |

## API Details

### createHash(algorithm)

Supports md5, sha1, sha256, sha384, and sha512. Returned Hash objects support update(data[, inputEncoding]) and digest([outputEncoding]).

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_CreateHash_And_RandomBytes` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_CreateHash_And_RandomBytes` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)

### createHmac(algorithm, key)

Supports md5, sha1, sha256, sha384, and sha512 with string/Buffer/ArrayBuffer/typed-array/DataView key material. Returned Hmac objects support update(data[, inputEncoding]) and digest([outputEncoding]).

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_CreateHmac` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_CreateHmac` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)

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

Supports Buffer, Uint8Array, and Int32Array. Additional Web Crypto support is limited to the documented subtle digest/import/sign/verify HMAC slice.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_GetRandomValues` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_GetRandomValues` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)

### webcrypto.subtle.digest(algorithm, data)

Supports SHA-1, SHA-256, SHA-384, and SHA-512 for Buffer, ArrayBuffer, TypedArray, and DataView input. Returns a Promise that resolves to an ArrayBuffer.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_ErrorPaths` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)

### webcrypto.subtle.importKey(format, keyData, algorithm, extractable, keyUsages)

Supports only importKey("raw", keyData, { name: "HMAC", hash: "SHA-1"|"SHA-256"|"SHA-384"|"SHA-512" }, extractable, ["sign"|"verify", ...]) for secret keys. Unsupported formats, algorithms, and key usages reject explicitly.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_ErrorPaths` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)

### webcrypto.subtle.sign(algorithm, key, data)

Supports HMAC CryptoKeys imported via importKey("raw", ...) for Buffer, ArrayBuffer, TypedArray, and DataView input. Returns a Promise that resolves to an ArrayBuffer signature.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_ErrorPaths` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)

### webcrypto.subtle.verify(algorithm, key, signature, data)

Supports HMAC CryptoKeys imported via importKey("raw", ...) and resolves to a boolean indicating whether the signature matches.

**Tests:**
- `Js2IL.Tests.Node.Crypto.ExecutionTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Crypto.GeneratorTests.Require_Crypto_WebCrypto_Subtle` (`Js2IL.Tests/Node/Crypto/GeneratorTests.cs`)
