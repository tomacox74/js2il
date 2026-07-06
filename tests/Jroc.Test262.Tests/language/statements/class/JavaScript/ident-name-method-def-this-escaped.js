var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  th\u0069s() { return 42; }
}

var obj = new C();

assert.sameValue(obj['this'](), 42, 'property exists');
