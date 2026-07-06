Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.throws(TypeError, function() {
  Map.prototype.clear.call(1);
});

assert.throws(TypeError, function() {
  Map.prototype.clear.call(true);
});

assert.throws(TypeError, function() {
  Map.prototype.clear.call('');
});

assert.throws(TypeError, function() {
  Map.prototype.clear.call(null);
});

assert.throws(TypeError, function() {
  Map.prototype.clear.call(undefined);
});

assert.throws(TypeError, function() {
  Map.prototype.clear.call(Symbol());
});
