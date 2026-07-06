assert.throws(TypeError, function() {
  WeakMap.prototype.set.call(undefined, {}, 1);
});

assert.throws(TypeError, function() {
  var map = new WeakMap();
  map.set.call(undefined, {}, 1);
});

