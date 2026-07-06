assert.sameValue(Set.prototype[Symbol.iterator], Set.prototype.values);
verifyProperty(Set.prototype, Symbol.iterator, {
  writable: true,
  enumerable: false,
  configurable: true,
});

