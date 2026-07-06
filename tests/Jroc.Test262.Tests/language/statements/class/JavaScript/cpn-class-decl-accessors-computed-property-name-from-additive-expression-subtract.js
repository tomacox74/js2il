var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

class C {
  get [1 - 1]() {
    return 0;
  }

  set [1 - 1](v) {
    return 0;
  }

  static get [1 - 1]() {
    return 0;
  }

  static set [1 - 1](v) {
    return 0;
  }
};

let c = new C();

assert.sameValue(
  c[1 - 1],
  0
);
assert.sameValue(
  c[1 - 1] = 0,
  0
);

assert.sameValue(
  C[1 - 1],
  0
);
assert.sameValue(
  C[1 - 1] = 0,
  0
);
assert.sameValue(
  c[String(1 - 1)],
  0
);
assert.sameValue(
  c[String(1 - 1)] = 0,
  0
);

assert.sameValue(
  C[String(1 - 1)],
  0
);
assert.sameValue(
  C[String(1 - 1)] = 0,
  0
);
