assert.sameValue(
  Number("1234.5678"),
  1234.5678,
  'Number("1234.5678") must return Number("1234") + (+("5678") * 1e-4)'
);
