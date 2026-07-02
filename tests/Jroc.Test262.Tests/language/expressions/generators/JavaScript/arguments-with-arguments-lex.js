// Upstream: test/language/expressions/generators/arguments-with-arguments-lex.js
function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };

var args;
var g = function* (x = args = arguments) {
  let arguments;
};

g().next();

assert.sameValue(typeof args, 'object');
assert.sameValue(args.length, 0);
