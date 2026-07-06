var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var callCount = 0;
var f;
f = function([arrow = () => {}]) {
  assert.sameValue(arrow.name, 'arrow');
  callCount = callCount + 1;
};

f([]);
assert.sameValue(callCount, 1, 'function invoked exactly once');
