assert.sameValue(isConstructor(Math.round), false, 'isConstructor(Math.round) must return false');

assert.throws(TypeError, () => {
  new Math.round();
});

