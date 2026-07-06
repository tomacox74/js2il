assert.throws(TypeError, () => {
  Error.prototype();
  throw new Test262Error();
});

