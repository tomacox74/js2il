for (var i = 2; i <= 36; i++) {
  assert.sameValue(parseInt("10$1", i), i, 'parseInt("10$1", i) must return the value of i');
}
