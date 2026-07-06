Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var m = new Map([
  ['a', 1],
  ['b', 2]
]);

var result = m.delete('a');

assert(result);
assert.sameValue(m.size, 1);
