assert.sameValue(isConstructor(Number.isNaN), false, 'isConstructor(Number.isNaN) must return false');

assert.throws(TypeError, () => {
  new Number.isNaN();
});

