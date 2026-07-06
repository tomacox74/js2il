assert.throws(TypeError, function() {
  WeakMap.prototype.set.call(null, {}, 1);
});

assert.throws(TypeError, function() {
  var map = new WeakMap();
  map.set.call(null, {}, 1);
});

