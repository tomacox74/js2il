var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

function Base() {}

Base.prototype = {
  set constructor(_) {
    throw new Test262Error("`Base.prototype.constructor` is unreachable.");
  }
};

class C extends Base {}

new C();
