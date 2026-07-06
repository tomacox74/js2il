Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map([
  ['foo', 'valid foo'],
  ['bar', false],
  ['baz', 'valid baz']
]);
map.set(0, false);
map.set(1, false);
map.set(2, 'valid 2');
map.delete(1);
map.delete('bar');

// Not setting a new key, just changing the value
map.set(0, 'valid 0');

var results = [];
var callback = function(value) {
  results.push(value);
};

map.forEach(callback);

assert.sameValue(results[0], 'valid foo');
assert.sameValue(results[1], 'valid baz');
assert.sameValue(results[2], 'valid 0');
assert.sameValue(results[3], 'valid 2');
assert.sameValue(results.length, 4);

map.clear();
results = [];

map.forEach(callback);
assert.sameValue(results.length, 0);
