var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var initCount = 0;

var callCount = 0;
var f;
f = function([[] = function() { initCount += 1; }()]) {
  assert.sameValue(initCount, 0);
  callCount = callCount + 1;
};

f([[23]]);
assert.sameValue(callCount, 1, 'function invoked exactly once');
