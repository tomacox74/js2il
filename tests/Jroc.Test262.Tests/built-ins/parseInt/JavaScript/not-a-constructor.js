assert.sameValue(isConstructor(parseInt), false, 'isConstructor(parseInt) must return false');

assert.throws(TypeError, () => {
  new parseInt(1);
});

