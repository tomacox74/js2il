var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  sup\u0065r() { return 42; }
}

var obj = new C();

assert.sameValue(obj['super'](), 42, 'property exists');
