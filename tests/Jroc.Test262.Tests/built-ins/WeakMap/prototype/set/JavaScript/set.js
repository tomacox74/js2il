assert.sameValue(
  typeof WeakMap.prototype.set,
  'function',
  'typeof WeakMap.prototype.set is "function"'
);

verifyProperty(WeakMap.prototype, 'set', {
  writable: true,
  enumerable: false,
  configurable: true,
});

