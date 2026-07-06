assert.sameValue(new Date(2016, 6).getMonth(), 6, 'first millisecond');
assert.sameValue(
  new Date(2016, 6, 0, 0, 0, 0, -1).getMonth(), 5, 'previous millisecond'
);
assert.sameValue(
  new Date(2016, 6, 31, 23, 59, 59, 999).getMonth(), 6, 'final millisecond'
);
assert.sameValue(
  new Date(2016, 6, 31, 23, 59, 59, 1000).getMonth(), 7, 'subsequent millisecond'
);

assert.sameValue(
  new Date(2016, 11, 31).getMonth(), 11, 'first millisecond (year boundary)'
);
assert.sameValue(
  new Date(2016, 11, 0, 0, 0, 0, -1).getMonth(),
  10,
  'previous millisecond (year boundary)'
);
assert.sameValue(
  new Date(2016, 11, 31, 23, 59, 59, 999).getMonth(),
  11,
  'final millisecond (year boundary)'
);
assert.sameValue(
  new Date(2016, 11, 31, 23, 59, 59, 1000).getMonth(),
  0,
  'subsequent millisecond (year boundary)'
);
