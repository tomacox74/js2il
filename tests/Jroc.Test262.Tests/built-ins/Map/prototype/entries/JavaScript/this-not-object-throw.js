Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.throws(TypeError, function() {
  Map.prototype.entries.call(false);
});

assert.throws(TypeError, function() {
  Map.prototype.entries.call(1);
});

assert.throws(TypeError, function() {
  Map.prototype.entries.call('');
});

assert.throws(TypeError, function() {
  Map.prototype.entries.call(undefined);
});

assert.throws(TypeError, function() {
  Map.prototype.entries.call(null);
});

assert.throws(TypeError, function() {
  Map.prototype.entries.call(Symbol());
});

assert.throws(TypeError, function() {
  var map = new Map();
  map.entries.call(false);
});
