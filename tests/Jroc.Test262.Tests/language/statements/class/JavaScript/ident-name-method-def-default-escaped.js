var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  def\u0061ult() { return 42; }
}

var obj = new C();

assert.sameValue(obj['default'](), 42, 'property exists');
