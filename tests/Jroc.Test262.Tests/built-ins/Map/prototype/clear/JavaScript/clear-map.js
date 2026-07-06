Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var m1 = new Map([
  ['foo', 'bar'],
  [1, 1]
]);
var m2 = new Map();
var m3 = new Map();
m2.set('foo', 'bar');
m2.set(1, 1);
m2.set(Symbol('a'), Symbol('a'));

m1.clear();
m2.clear();
m3.clear();

assert.sameValue(m1.size, 0);
assert.sameValue(m2.size, 0);
assert.sameValue(m3.size, 0);
