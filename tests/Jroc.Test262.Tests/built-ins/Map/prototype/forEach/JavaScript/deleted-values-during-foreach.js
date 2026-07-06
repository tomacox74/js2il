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
    map.delete('bar');
  }
  results.push({
    value: value,
    key: key
  });
  count++;
});

assert.sameValue(results.length, 1);
assert.sameValue(results[0].key, 'foo');
assert.sameValue(results[0].value, 0);
