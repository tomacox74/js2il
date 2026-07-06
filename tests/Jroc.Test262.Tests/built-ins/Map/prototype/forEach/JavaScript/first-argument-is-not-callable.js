Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map();

assert.throws(TypeError, function() {
  map.forEach({});
});

assert.throws(TypeError, function() {
  map.forEach([]);
});

assert.throws(TypeError, function() {
  map.forEach(1);
});

assert.throws(TypeError, function() {
  map.forEach('');
});

assert.throws(TypeError, function() {
  map.forEach(null);
});

assert.throws(TypeError, function() {
  map.forEach(undefined);
});

assert.throws(TypeError, function() {
  map.forEach(Symbol());
});
