var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  \u0065xtends() { return 42; }
}

var obj = new C();

assert.sameValue(obj['extends'](), 42, 'property exists');
