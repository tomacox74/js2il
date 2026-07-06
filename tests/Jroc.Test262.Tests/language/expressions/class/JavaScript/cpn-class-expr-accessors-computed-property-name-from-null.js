var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let C = class {
  get [null]() {
    return null;
  }

  set [null](v) {
    return null;
  }

  static get [null]() {
    return null;
  }

  static set [null](v) {
    return null;
  }
};

let c = new C();

assert.sameValue(
  c[null],
  null
);
assert.sameValue(
  c[null] = null,
  null
);

assert.sameValue(
  C[null],
  null
);
assert.sameValue(
  C[null] = null,
  null
);
assert.sameValue(
  c[String(null)],
  null
);
assert.sameValue(
  c[String(null)] = null,
  null
);

assert.sameValue(
  C[String(null)],
  null
);
assert.sameValue(
  C[String(null)] = null,
  null
);
