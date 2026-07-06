var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  l\u0065t() { return 42; }
}

var obj = new C();

assert.sameValue(obj['let'](), 42, 'property exists');
