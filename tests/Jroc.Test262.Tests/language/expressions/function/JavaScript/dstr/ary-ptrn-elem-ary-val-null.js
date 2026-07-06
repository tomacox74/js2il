var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var f = function([[x]]) {};

assert.throws(TypeError, function() {
  f([null]);
});
