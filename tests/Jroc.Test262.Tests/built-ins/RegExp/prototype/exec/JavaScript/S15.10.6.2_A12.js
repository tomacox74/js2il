(/foo/).test('xfoox');
var match = new RegExp('(.|\r|\n)*','').exec()[0];
assert.notSameValue(match, 'xfoox', 'The value of match is not "xfoox"');
assert.sameValue(match, 'undefined', 'The value of match is expected to be "undefined"');
