var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

let x = 1;



let C = class {
  get [x]() {
    return '2';
  }

  set [x](v) {
    return '2';
  }

  static get [x]() {
    return '2';
  }

  static set [x](v) {
    return '2';
  }
};

let c = new C();

assert.sameValue(
  c[x],
  '2'
);
assert.sameValue(
  c[x] = '2',
  '2'
);

assert.sameValue(
  C[x],
  '2'
);
assert.sameValue(
  C[x] = '2',
  '2'
);
assert.sameValue(
  c[String(x)],
  '2'
);
assert.sameValue(
  c[String(x)] = '2',
  '2'
);

assert.sameValue(
  C[String(x)],
  '2'
);
assert.sameValue(
  C[String(x)] = '2',
  '2'
);
