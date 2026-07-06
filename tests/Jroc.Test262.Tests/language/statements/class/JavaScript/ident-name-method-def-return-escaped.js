var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  r\u0065turn() { return 42; }
}

var obj = new C();

assert.sameValue(obj['return'](), 42, 'property exists');
