var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var values = [2, 1, 3];

var callCount = 0;
var f;
f = ([[...x] = values]) => {
  assert(Array.isArray(x));
  assert.sameValue(x[0], 2);
  assert.sameValue(x[1], 1);
  assert.sameValue(x[2], 3);
  assert.sameValue(x.length, 3);
  assert.notSameValue(x, values);
  callCount = callCount + 1;
};

f([]);
assert.sameValue(callCount, 1, 'arrow function invoked exactly once');
