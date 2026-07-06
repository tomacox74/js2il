var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var callCount = 0;
var f;
f = ([cover = (function () {}), xCover = (0, function() {})]) => {
  assert.sameValue(cover.name, 'cover');
  assert.notSameValue(xCover.name, 'xCover');
  callCount = callCount + 1;
};

f([]);
assert.sameValue(callCount, 1, 'arrow function invoked exactly once');
