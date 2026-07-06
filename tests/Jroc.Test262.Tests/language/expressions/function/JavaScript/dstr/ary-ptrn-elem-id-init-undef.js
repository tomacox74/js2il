var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var callCount = 0;
var f;
f = function([x = 23]) {
  assert.sameValue(x, 23);
  callCount = callCount + 1;
};

f([undefined]);
assert.sameValue(callCount, 1, 'function invoked exactly once');
