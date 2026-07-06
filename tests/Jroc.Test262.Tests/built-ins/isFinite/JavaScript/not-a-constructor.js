assert.sameValue(isConstructor(isFinite), false, 'isConstructor(isFinite) must return false');

assert.throws(TypeError, () => {
  new isFinite(1);
});

