Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var count = 0;
var nextItem;
var iterable = {};
iterable[Symbol.iterator] = function() {
  return {
    next: function() {
      return {
        value: nextItem,
        done: false
      };
    },
    return: function() {
      count += 1;
    }
  };
};

nextItem = 1;
assert.throws(TypeError, function() {
  new Map(iterable);
});
assert.sameValue(count, 1);

nextItem = true;
assert.throws(TypeError, function() {
  new Map(iterable);
});
assert.sameValue(count, 2);

nextItem = '';
assert.throws(TypeError, function() {
  new Map(iterable);
});
assert.sameValue(count, 3);

nextItem = null;
assert.throws(TypeError, function() {
  new Map(iterable);
});
assert.sameValue(count, 4);

nextItem = undefined;
assert.throws(TypeError, function() {
  new Map(iterable);
});
assert.sameValue(count, 5);

nextItem = Symbol('a');
assert.throws(TypeError, function() {
  new Map(iterable);
});
assert.sameValue(count, 6);
