var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let x = 0;


class C {
  get [x || 1]() {
    return 2;
  }

  set [x || 1](v) {
    return 2;
  }

  static get [x || 1]() {
    return 2;
  }

  static set [x || 1](v) {
    return 2;
  }
};

let c = new C();

assert.sameValue(
  c[x || 1],
  2
);
assert.sameValue(
  c[x || 1] = 2,
  2
);

assert.sameValue(
  C[x || 1],
  2
);
assert.sameValue(
  C[x || 1] = 2,
  2
);
assert.sameValue(
  c[String(x || 1)],
  2
);
assert.sameValue(
  c[String(x || 1)] = 2,
  2
);

assert.sameValue(
  C[String(x || 1)],
  2
);
assert.sameValue(
  C[String(x || 1)] = 2,
  2
);

assert.sameValue(x, 0);
