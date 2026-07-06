assert.sameValue(new Date(2016, 0).getFullYear(), 2016, 'first millisecond');
assert.sameValue(
  new Date(2016, 0, 1, 0, 0, 0, -1).getFullYear(), 2015, 'previous millisecond'
);
assert.sameValue(
  new Date(2016, 11, 31, 23, 59, 59, 999).getFullYear(),
  2016,
  'final millisecond'
);
assert.sameValue(
  new Date(2016, 11, 31, 23, 59, 59, 1000).getFullYear(),
  2017,
  'subsequent millisecond'
);
