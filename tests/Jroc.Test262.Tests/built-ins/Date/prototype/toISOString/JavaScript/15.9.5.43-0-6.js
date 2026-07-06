assert.throws(TypeError, function() {
  Date.prototype.toISOString.call([]);
});
