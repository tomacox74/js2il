assert.throws(TypeError, function() {
  WeakSet.prototype.has.call(null, {});
});

assert.throws(TypeError, function() {
  var s = new WeakSet();
  s.has.call(null, {});
});

