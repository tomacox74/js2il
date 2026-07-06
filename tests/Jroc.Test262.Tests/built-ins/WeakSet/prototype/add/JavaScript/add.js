assert.sameValue(
  typeof WeakSet.prototype.add,
  'function',
  'typeof WeakSet.prototype.add is "function"'
);

verifyProperty(WeakSet.prototype, 'add', {
  writable: true,
  enumerable: false,
  configurable: true,
});

