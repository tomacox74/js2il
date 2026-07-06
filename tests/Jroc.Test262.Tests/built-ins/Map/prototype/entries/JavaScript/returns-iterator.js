Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map();
map.set('a',1);
map.set('b',2);
map.set('c',3);

var iterator = map.entries();
var result;

result = iterator.next();
assert.sameValue(result.value[0], 'a', 'First result `value` ("key")');
assert.sameValue(result.value[1], 1, 'First result `value` ("value")');
assert.sameValue(result.value.length, 2, 'First result `value` (length)');
assert.sameValue(result.done, false, 'First result `done` flag');

result = iterator.next();
assert.sameValue(result.value[0], 'b', 'Second result `value` ("key")');
assert.sameValue(result.value[1], 2, 'Second result `value` ("value")');
assert.sameValue(result.value.length, 2, 'Second result `value` (length)');
assert.sameValue(result.done, false, 'Second result `done` flag');

result = iterator.next();
assert.sameValue(result.value[0], 'c', 'Third result `value` ("key")');
assert.sameValue(result.value[1], 3, 'Third result `value` ("value")');
assert.sameValue(result.value.length, 2, 'Third result `value` (length)');
assert.sameValue(result.done, false, 'Third result `done` flag');

result = iterator.next();
assert.sameValue(result.value, undefined, 'Exhausted result `value`');
assert.sameValue(result.done, true, 'Exhausted result `done` flag');

result = iterator.next();
assert.sameValue(
  result.value, undefined, 'Exhausted result `value` (repeated request)'
);
assert.sameValue(
  result.done, true, 'Exhausted result `done` flag (repeated request)'
);
