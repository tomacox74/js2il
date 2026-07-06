var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

var thrower = function() {
  throw new Test262Error();
};


assert.throws(Test262Error, function() {
  0, class {
    get [thrower()]() {}
  };
}, '`get` accessor');

assert.throws(Test262Error, function() {
  0, class {
    set [thrower()](_) {}
  };
}, '`set` accessor');
