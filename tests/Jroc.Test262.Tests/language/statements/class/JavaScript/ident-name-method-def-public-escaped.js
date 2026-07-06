var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  pu\u0062lic() { return 42; }
}

var obj = new C();

assert.sameValue(obj['public'](), 42, 'property exists');
