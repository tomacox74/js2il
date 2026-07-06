var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

var args;

class A {
  constructor() {
    args = arguments;
  }
}

class B extends A {
  /*
    The missing constructor is created by the runtime:

    constructor(...args) {
      super(...args);
    }

   */
}

new B(0, 1, 2);


assert.sameValue(args[0], 0);
assert.sameValue(args[1], 1);
assert.sameValue(args[2], 2);

