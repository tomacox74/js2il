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
  ['b', 2],
  ['c', 3]
]);
var e = m.entries();

e.next();
m.delete('b');

var n = e.next();

assert.sameValue(n.value[0], 'c');
assert.sameValue(n.value[1], 3);

n = e.next();
assert.sameValue(n.value, undefined);
assert.sameValue(n.done, true);
