assert.sameValue(WeakMap.prototype.constructor, WeakMap);
assert.sameValue((new WeakMap()).constructor, WeakMap);

verifyProperty(WeakMap.prototype, 'constructor', {
  writable: true,
  enumerable: false,
  configurable: true,
});

