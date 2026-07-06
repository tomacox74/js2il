assert.sameValue(isConstructor(Math.ceil), false, 'isConstructor(Math.ceil) must return false');

assert.throws(TypeError, () => {
  new Math.ceil();
});

