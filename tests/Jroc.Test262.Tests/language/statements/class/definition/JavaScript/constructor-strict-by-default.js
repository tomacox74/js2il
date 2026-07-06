var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  constructor() {
    assert.throws(ReferenceError, function() {
      nonExistingBinding = 42;
    });
  }
}
new C();
