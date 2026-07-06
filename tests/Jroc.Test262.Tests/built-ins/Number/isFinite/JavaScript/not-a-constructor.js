assert.sameValue(isConstructor(Number.isFinite), false, 'isConstructor(Number.isFinite) must return false');

assert.throws(TypeError, () => {
  new Number.isFinite();
});

