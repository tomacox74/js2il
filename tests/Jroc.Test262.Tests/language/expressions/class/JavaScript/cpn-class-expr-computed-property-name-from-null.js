var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let C = class {
  [null]() {
    return null;
  }
  static [null]() {
    return null;
  }
};

let c = new C();

assert.sameValue(
  c[null](),
  null
);
assert.sameValue(
  C[null](),
  null
);
assert.sameValue(
  c[String(null)](),
  null
);
assert.sameValue(
  C[String(null)](),
  null
);
