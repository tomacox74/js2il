Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map();
var iterator = map.entries();
var result = iterator.next();

assert.sameValue(
  result.value, undefined,
  'The value of `result.value` is `undefined`'
);
assert.sameValue(result.done, true, 'The value of `result.done` is `true`');
