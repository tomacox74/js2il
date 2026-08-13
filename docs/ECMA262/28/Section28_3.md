<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.3: Module Namespace Objects

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-13T01:31:23Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 28.3 | Module Namespace Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 28.3.1 | %Symbol.toStringTag% | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 28.3 ([tc39.es](https://tc39.es/ecma262/#sec-module-namespace-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Module namespace exotic objects | Not Yet Supported | [`enumerate-binding-uninit.js`](../../../tests/Jroc.Test262.Tests/language/module-code/namespace/internals/JavaScript/enumerate-binding-uninit.js) | `test/language/module-code/namespace/internals/enumerate-binding-uninit.js` | Full ECMAScript module namespace exotic objects are not implemented. Static-module interop does preserve the uninitialized live-binding ReferenceError when for-in enumerates a self namespace, but it is not a complete 28.3 implementation. |

