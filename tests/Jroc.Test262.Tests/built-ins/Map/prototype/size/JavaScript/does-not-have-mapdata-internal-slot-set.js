var descriptor = Object.getOwnPropertyDescriptor(Map.prototype, 'size');

var map = new Map();

// Does not throw
descriptor.get.call(map);

assert.throws(TypeError, function() {
  descriptor.get.call(new Set());
});

