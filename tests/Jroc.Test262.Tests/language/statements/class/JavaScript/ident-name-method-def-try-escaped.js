var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  tr\u0079() { return 42; }
}

var obj = new C();

assert.sameValue(obj['try'](), 42, 'property exists');
