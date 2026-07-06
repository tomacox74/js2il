Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.throws(TypeError, function() {
  new Map([1]);
});

assert.throws(TypeError, function() {
  new Map(['']);
});

assert.throws(TypeError, function() {
  new Map([true]);
});

assert.throws(TypeError, function() {
  new Map([null]);
});

assert.throws(TypeError, function() {
  new Map([Symbol('a')]);
});

assert.throws(TypeError, function() {
  new Map([undefined]);
});

assert.throws(TypeError, function() {
  new Map([
    ['a', 1],
    2
  ]);
});
