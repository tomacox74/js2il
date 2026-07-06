"use strict";

Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var _this = [];
var map = new Map();

map.set(0, 0);
map.set(1, 1);
map.set(2, 2);

map.forEach(function() {
  _this.push(this);
});

assert.sameValue(_this[0], undefined);
assert.sameValue(_this[1], undefined);
assert.sameValue(_this[2], undefined);
