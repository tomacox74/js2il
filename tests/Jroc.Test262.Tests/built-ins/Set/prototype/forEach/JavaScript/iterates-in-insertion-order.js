var s = new Set();
var expects = [1, 2, 3];

s.add(1).add(2).add(3);

s.forEach(function(value, entry, set) {
  var expect = expects.shift();

  assert.sameValue(value, expect);
  assert.sameValue(entry, expect);
  assert.sameValue(set, s);
});

assert.sameValue(expects.length, 0, "The value of `expects.length` is `0`");

