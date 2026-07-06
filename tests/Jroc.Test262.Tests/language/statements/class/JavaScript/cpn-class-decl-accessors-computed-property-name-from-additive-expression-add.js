var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  get [1 + 1]() {
    return 2;
  }

  set [1 + 1](v) {
    return 2;
  }

  static get [1 + 1]() {
    return 2;
  }

  static set [1 + 1](v) {
    return 2;
  }
};

let c = new C();

assert.sameValue(
  c[1 + 1],
  2
);
assert.sameValue(
  c[1 + 1] = 2,
  2
);

assert.sameValue(
  C[1 + 1],
  2
);
assert.sameValue(
  C[1 + 1] = 2,
  2
);
assert.sameValue(
  c[String(1 + 1)],
  2
);
assert.sameValue(
  c[String(1 + 1)] = 2,
  2
);

assert.sameValue(
  C[String(1 + 1)],
  2
);
assert.sameValue(
  C[String(1 + 1)] = 2,
  2
);
