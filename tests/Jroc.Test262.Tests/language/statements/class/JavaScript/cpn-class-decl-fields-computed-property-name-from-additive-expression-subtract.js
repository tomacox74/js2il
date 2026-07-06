var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let C = class {
  [1 - 1] = 0;

  static [1 - 1] = 0;
};

let c = new C();

assert.sameValue(
  c[1 - 1],
  0
);
assert.sameValue(
  C[1 - 1],
  0
);
assert.sameValue(
  c[String(1 - 1)],
  0
);
assert.sameValue(
  C[String(1 - 1)],
  0
);
