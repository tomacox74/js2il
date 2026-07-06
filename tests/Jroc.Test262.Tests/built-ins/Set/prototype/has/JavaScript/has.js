assert.sameValue(
  typeof Set.prototype.has,
  "function",
  "`typeof Set.prototype.has` is `'function'`"
);

verifyProperty(Set.prototype, "has", {
  writable: true,
  enumerable: false,
  configurable: true,
});

