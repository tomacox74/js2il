var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  i\u0066() { return 42; }
}

var obj = new C();

assert.sameValue(obj['if'](), 42, 'property exists');
