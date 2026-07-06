var descriptor = Object.getOwnPropertyDescriptor(Set.prototype, "size");

assert.sameValue(
  typeof descriptor.get,
  "function",
  "`typeof descriptor.get` is `'function'`"
);
assert.sameValue(
  typeof descriptor.set,
  "undefined",
  "`typeof descriptor.set` is `\"undefined\"`"
);

verifyNotEnumerable(Set.prototype, "size");
verifyConfigurable(Set.prototype, "size");

