Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var m = new Map([
  [1, 1],
  [2, 2],
  [3, 3]
]);
var e = m.entries();

e.next();
m.clear();

var n = e.next();
assert.sameValue(n.value, undefined);
assert.sameValue(n.done, true);
