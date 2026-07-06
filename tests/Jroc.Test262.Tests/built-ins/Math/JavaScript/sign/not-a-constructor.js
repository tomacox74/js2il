assert.sameValue(isConstructor(Math.sign), false, 'isConstructor(Math.sign) must return false');

assert.throws(TypeError, () => {
  new Math.sign();
});

