var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

assert.throws(ReferenceError, function() {
  class C {
    static get [test262unresolvable]() {}
  }
}, '`get` accessor');

assert.throws(ReferenceError, function() {
  class C {
    static set [test262unresolvable](_) {}
  }
}, '`set` accessor');
