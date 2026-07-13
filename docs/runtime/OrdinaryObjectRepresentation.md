# Ordinary object representation

`JsObject` is JROC's runtime representation for ordinary JavaScript objects. It
provides shape/slot storage, deterministic own-key ordering, unboxed
number/boolean slots, descriptor integration, and prototype support. Compiler
object literals, function-constructed instances, built-in result records,
module records, and Node-created ordinary records use this representation.

Runtime property operations use `ObjectRuntime` and `Object` so descriptors,
prototypes, accessors, integrity levels, proxies, and enumeration retain
ECMAScript behavior. Runtime-created descriptor records are also `JsObject`
instances; callers must not depend on a CLR dynamic-object implementation.

Two intentionally different boundaries remain:

- `Array` retains its dedicated exotic element and prototype representation
  until [#1443](https://github.com/tomacox74/js2il/issues/1443) makes it a
  `JsObject` subclass without weakening array index or `length` semantics.
- C# dynamic interoperability is a hosting concern. `JsObject` still carries
  transitional DLR support while [#1461](https://github.com/tomacox74/js2il/issues/1461)
  moves it to `JsDynamicValueProxy` and `JsDynamicExports`. Internal JavaScript
  execution does not dispatch through the DLR.

External CLR dictionaries and POCOs remain host objects. They are supported
through their normal host-object paths and are not treated as runtime-owned
ordinary JavaScript objects.
