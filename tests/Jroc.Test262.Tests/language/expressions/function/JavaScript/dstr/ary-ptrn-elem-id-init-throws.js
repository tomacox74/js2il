var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var f = function([x = (function() { throw new Test262Error(); })()]) {};

assert.throws(Test262Error, function() {
  f([undefined]);
});
