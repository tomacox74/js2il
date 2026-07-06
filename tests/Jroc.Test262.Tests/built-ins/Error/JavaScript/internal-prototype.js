assert.sameValue(
  Function.prototype.isPrototypeOf(Error().constructor),
  true,
  'Function.prototype.isPrototypeOf(err1.constructor) returns true'
);

assert.sameValue(
  Function.prototype.isPrototypeOf(Error.constructor),
  true,
  'Function.prototype.isPrototypeOf(Error.constructor) returns true'
);

