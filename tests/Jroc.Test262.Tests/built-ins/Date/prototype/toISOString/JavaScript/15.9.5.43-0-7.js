assert.throws(TypeError, function() {
  Date.prototype.toISOString.call(15);
});
