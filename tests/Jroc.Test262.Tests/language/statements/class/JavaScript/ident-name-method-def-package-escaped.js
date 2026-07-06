var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  p\u0061ckage() { return 42; }
}

var obj = new C();

assert.sameValue(obj['package'](), 42, 'property exists');
