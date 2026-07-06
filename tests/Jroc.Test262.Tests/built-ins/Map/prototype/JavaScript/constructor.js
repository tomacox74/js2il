assert.sameValue(Map.prototype.constructor, Map);
assert.sameValue((new Map()).constructor, Map);

verifyProperty(Map.prototype, 'constructor', {
  writable: true,
  enumerable: false,
  configurable: true,
});

