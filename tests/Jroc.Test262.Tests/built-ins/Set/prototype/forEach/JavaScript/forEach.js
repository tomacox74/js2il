assert.sameValue(
  typeof Set.prototype.forEach,
  "function",
  "`typeof Set.prototype.forEach` is `'function'`"
);

verifyProperty(Set.prototype, "forEach", {
  writable: true,
  enumerable: false,
  configurable: true,
});

