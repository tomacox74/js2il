var map = new Map([
  [0, undefined],
  [undefined, undefined],
  [false, undefined],
  [NaN, undefined],
  [null, undefined],
  ['', undefined],
  [Symbol(), undefined],
]);

assert.sameValue(map.size, 7, 'The value of `map.size` is `7`');

