var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  n\u0065w() { return 42; }
}

var obj = new C();

assert.sameValue(obj['new'](), 42, 'property exists');
