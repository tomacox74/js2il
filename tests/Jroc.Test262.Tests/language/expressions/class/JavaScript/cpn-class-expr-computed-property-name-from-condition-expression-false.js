var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let C = class {
  [false ? 1 : 2]() {
    return 1;
  }
  static [false ? 1 : 2]() {
    return 1;
  }
};

let c = new C();

assert.sameValue(
  c[false ? 1 : 2](),
  1
);
assert.sameValue(
  C[false ? 1 : 2](),
  1
);
assert.sameValue(
  c[String(false ? 1 : 2)](),
  1
);
assert.sameValue(
  C[String(false ? 1 : 2)](),
  1
);
