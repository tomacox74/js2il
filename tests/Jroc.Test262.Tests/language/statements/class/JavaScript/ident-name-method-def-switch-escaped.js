var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  sw\u0069tch() { return 42; }
}

var obj = new C();

assert.sameValue(obj['switch'](), 42, 'property exists');
