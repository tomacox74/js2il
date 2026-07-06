var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var callCount = 0;
var f;
f = ([{ x, y, z } = { x: 44, y: 55, z: 66 }]) => {
  assert.sameValue(x, 11);
  assert.sameValue(y, 22);
  assert.sameValue(z, 33);
  callCount = callCount + 1;
};

f([{ x: 11, y: 22, z: 33 }]);
assert.sameValue(callCount, 1, 'arrow function invoked exactly once');
