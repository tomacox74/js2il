Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var expectedThis = {};
var _this = [];

var map = new Map();
map.set(0, 0);
map.set(1, 1);
map.set(2, 2);

var callback = function() {
  _this.push(this);
};

map.forEach(callback, expectedThis);

assert.sameValue(_this[0], expectedThis);
assert.sameValue(_this[1], expectedThis);
assert.sameValue(_this[2], expectedThis);
