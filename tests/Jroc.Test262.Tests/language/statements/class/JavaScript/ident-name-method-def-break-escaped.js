var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  bre\u0061k() { return 42; }
}

var obj = new C();

assert.sameValue(obj['break'](), 42, 'property exists');
