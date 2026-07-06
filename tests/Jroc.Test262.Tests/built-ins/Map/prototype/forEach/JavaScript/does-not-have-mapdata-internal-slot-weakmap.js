Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.throws(TypeError, function() {
  Map.prototype.forEach.call(new WeakMap(), function() {});
});

assert.throws(TypeError, function() {
  var m = new Map();
  m.forEach.call(new WeakMap(), function() {});
});
