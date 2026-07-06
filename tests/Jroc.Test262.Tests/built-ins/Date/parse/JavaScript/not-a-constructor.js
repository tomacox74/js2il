assert.sameValue(isConstructor(Date.parse), false, 'isConstructor(Date.parse) must return false');

assert.throws(TypeError, () => {
  new Date.parse();
});

