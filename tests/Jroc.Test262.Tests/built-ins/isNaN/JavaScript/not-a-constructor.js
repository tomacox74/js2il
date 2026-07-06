assert.sameValue(isConstructor(isNaN), false, 'isConstructor(isNaN) must return false');

assert.throws(TypeError, () => {
  new isNaN(1);
});

