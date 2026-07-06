Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map();
map.set('foo', 42);
map.set('bar', 'baz');

var results = [];

var callback = function(value, key, thisArg) {
  results.push({
    value: value,
    key: key,
    thisArg: thisArg
  });
};

map.forEach(callback);

assert.sameValue(results[0].value, 42);
assert.sameValue(results[0].key, 'foo');
assert.sameValue(results[0].thisArg, map);

assert.sameValue(results[1].value, 'baz');
assert.sameValue(results[1].key, 'bar');
assert.sameValue(results[1].thisArg, map);

assert.sameValue(results.length, 2);
