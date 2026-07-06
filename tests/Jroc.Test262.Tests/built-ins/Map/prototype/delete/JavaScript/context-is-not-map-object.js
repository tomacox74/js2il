Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.throws(TypeError, function() {
  Map.prototype.delete.call({}, 'attr');
});

assert.throws(TypeError, function() {
  Map.prototype.delete.call([], 'attr');
});
