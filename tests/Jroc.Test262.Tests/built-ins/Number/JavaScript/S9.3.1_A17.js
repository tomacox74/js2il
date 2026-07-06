assert.sameValue(Number("1"), 1, 'Number("1") must return 1');
assert.sameValue(Number("0x1"), 1, 'Number("0x1") must return 1');
assert.sameValue(+("0X1"), 1, 'The value of `+("0X1")` is expected to be 1');
