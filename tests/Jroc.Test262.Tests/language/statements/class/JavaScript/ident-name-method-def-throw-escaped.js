var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  t\u0068row() { return 42; }
}

var obj = new C();

assert.sameValue(obj['throw'](), 42, 'property exists');
