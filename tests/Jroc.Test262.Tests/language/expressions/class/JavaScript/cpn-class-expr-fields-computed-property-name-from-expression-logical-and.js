var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let x = 0;


let C = class {
  [x && 1] = 2;

  static [x && 1] = 2;
};

let c = new C();

assert.sameValue(
  c[x && 1],
  2
);
assert.sameValue(
  C[x && 1],
  2
);
assert.sameValue(
  c[String(x && 1)],
  2
);
assert.sameValue(
  C[String(x && 1)],
  2
);

assert.sameValue(x, 0);
