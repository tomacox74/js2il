Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map();

var result = map.forEach(function() {
  return true;
});

assert.sameValue(result, undefined, 'Empty map#forEach returns undefined');

map.set(1, 1);
result = map.forEach(function() {
  return true;
});

assert.sameValue(result, undefined, 'map#forEach returns undefined');
