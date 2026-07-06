Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

Map.prototype.set = null;

assert.throws(TypeError, function() {
  new Map([
    [1, 1],
    [2, 2]
  ]);
});
