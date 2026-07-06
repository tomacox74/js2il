var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  [true ? 1 : 2]() {
    return 2;
  }
  static [true ? 1 : 2]() {
    return 2;
  }
};

let c = new C();

assert.sameValue(
  c[true ? 1 : 2](),
  2
);
assert.sameValue(
  C[true ? 1 : 2](),
  2
);
assert.sameValue(
  c[String(true ? 1 : 2)](),
  2
);
assert.sameValue(
  C[String(true ? 1 : 2)](),
  2
);
