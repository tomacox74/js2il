var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

function Base() {}

Base.prototype = {
  set m(_) {
    throw new Test262Error("`Base.prototype.m` is unreachable.");
  }
};

class C extends Base {
  m() {
    return 1;
  }
}

assert.sameValue(new C().m(), 1, "`new C().m()` returns `1`");
