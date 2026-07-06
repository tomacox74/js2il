var s = new Set([1]);
var expects = [1, 2, 3];

s.forEach(function(value, entry, set) {
  var expect = expects.shift();

  if (value === 1) {
    set.add(2);
  }

  if (value === 2) {
    set.add(3);
  }

  assert.sameValue(value, expect);
  assert.sameValue(entry, expect);
  assert.sameValue(set, s);
});

assert.sameValue(expects.length, 0, "The value of `expects.length` is `0`");

