var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var initCount = 0;
function counter() {
  initCount += 1;
}

var callCount = 0;
var f;
f = function([w = counter(), x = counter(), y = counter(), z = counter()]) {
  assert.sameValue(w, null);
  assert.sameValue(x, 0);
  assert.sameValue(y, false);
  assert.sameValue(z, '');
  assert.sameValue(initCount, 0);
  callCount = callCount + 1;
};

f([null, 0, false, '']);
assert.sameValue(callCount, 1, 'function invoked exactly once');
