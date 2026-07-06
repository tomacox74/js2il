Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var map = new Map([[0, 0]]);

assert.throws(Test262Error, function() {
  map.forEach(function() {
    throw new Test262Error();
  });
});
