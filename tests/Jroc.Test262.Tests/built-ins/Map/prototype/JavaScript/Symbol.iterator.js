assert.sameValue(Map.prototype[Symbol.iterator], Map.prototype.entries);
verifyProperty(Map.prototype, Symbol.iterator, {
  writable: true,
  enumerable: false,
  configurable: true,
});

