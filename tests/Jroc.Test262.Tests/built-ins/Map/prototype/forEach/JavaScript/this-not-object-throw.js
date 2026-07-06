Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.throws(TypeError, function() {
  Map.prototype.forEach.call(false, function() {});
});

assert.throws(TypeError, function() {
  Map.prototype.forEach.call(1, function() {});
});

assert.throws(TypeError, function() {
  Map.prototype.forEach.call('', function() {});
});

assert.throws(TypeError, function() {
  Map.prototype.forEach.call(undefined, function() {});
});

assert.throws(TypeError, function() {
  Map.prototype.forEach.call(null, function() {});
});

assert.throws(TypeError, function() {
  Map.prototype.forEach.call(Symbol(), function() {});
});

assert.throws(TypeError, function() {
  var map = new Map();
  map.forEach.call(false, function() {});
});
