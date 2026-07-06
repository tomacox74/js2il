var descriptor = Object.getOwnPropertyDescriptor(Map.prototype, 'size');

assert.throws(TypeError, function() {
  descriptor.get.call(1);
});

assert.throws(TypeError, function() {
  descriptor.get.call(false);
});

assert.throws(TypeError, function() {
  descriptor.get.call(1);
});

assert.throws(TypeError, function() {
  descriptor.get.call('');
});

assert.throws(TypeError, function() {
  descriptor.get.call(undefined);
});

assert.throws(TypeError, function() {
  descriptor.get.call(null);
});

assert.throws(TypeError, function() {
  descriptor.get.call(Symbol());
});

