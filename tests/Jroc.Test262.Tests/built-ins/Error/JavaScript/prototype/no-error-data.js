Object.defineProperty(Error.prototype, Symbol.toStringTag, {
  value: null,
});

assert.sameValue(
  Object.prototype.toString.call(Error.prototype),
  "[object Object]"
);

