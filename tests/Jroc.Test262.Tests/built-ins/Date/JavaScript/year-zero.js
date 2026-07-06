const invalidStrings = [
  "-000000-03-31T00:45Z",
  "-000000-03-31T01:45",
  "-000000-03-31T01:45:00+01:00"
];

for (const str of invalidStrings) {
  assert.sameValue(+new Date(str), NaN, "reject minus zero as extended year");
}
