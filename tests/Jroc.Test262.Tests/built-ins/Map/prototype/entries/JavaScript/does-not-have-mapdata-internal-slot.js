Test262Error.prototype.name = "Test262Error";

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

var m = new Map();

assert.throws(TypeError, function() {
  Map.prototype.entries.call([]);
});

assert.throws(TypeError, function() {
  m.entries.call([]);
});

assert.throws(TypeError, function() {
  Map.prototype.entries.call({});
});

assert.throws(TypeError, function() {
  m.entries.call({});
});
