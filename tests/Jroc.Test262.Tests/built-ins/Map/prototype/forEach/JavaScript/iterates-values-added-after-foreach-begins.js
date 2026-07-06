Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map();
map.set('foo', 0);
map.set('bar', 1);

var count = 0;
var results = [];

map.forEach(function(value, key) {
  if (count === 0) {
    map.set('baz', 2);
  }
  results.push({
    value: value,
    key: key
  });
  count++;
});

assert.sameValue(count, 3);
assert.sameValue(map.size, 3);

assert.sameValue(results[0].key, 'foo');
assert.sameValue(results[0].value, 0);

assert.sameValue(results[1].key, 'bar');
assert.sameValue(results[1].value, 1);

assert.sameValue(results[2].key, 'baz');
assert.sameValue(results[2].value, 2);
