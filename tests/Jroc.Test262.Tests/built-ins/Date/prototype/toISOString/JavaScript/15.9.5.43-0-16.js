var date = new String("1970-01-00000:00:00.000Z");
assert.throws(TypeError, function() {
  Date.prototype.toISOString.call(date);
});
