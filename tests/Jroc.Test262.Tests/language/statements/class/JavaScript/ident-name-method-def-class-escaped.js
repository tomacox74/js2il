var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  cl\u0061ss() { return 42; }
}

var obj = new C();

assert.sameValue(obj['class'](), 42, 'property exists');
