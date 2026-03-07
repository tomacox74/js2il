<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.5: Abstract Operations for Keyed Collections

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T01:50:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.5 | Abstract Operations for Keyed Collections | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-keyed-collections) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.5.1 | CanonicalizeKeyedCollectionKey ( key ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-canonicalizekeyedcollectionkey) |

## Support

Feature-level support tracking with test script references.

### 24.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-canonicalizekeyedcollectionkey))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CanonicalizeKeyedCollectionKey behavior | Incomplete | [`Map_Null_Key.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Null_Key.js) | Map uses a null sentinel plus a SameValueZero-style comparer to support common key cases such as null and NaN, but JS2IL does not implement a shared CanonicalizeKeyedCollectionKey abstraction across Map, Set, WeakMap, and WeakSet, and full canonical storage behavior such as normalizing -0 to +0 in observable results remains incomplete. |

