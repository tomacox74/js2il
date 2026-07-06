assert.throws(TypeError, () => {
  new Error.prototype();
  throw new Test262Error();
});

