// Upstream: test/language/expressions/generators/arguments-with-arguments-lex.js
var args;
var g = function* (x = args = arguments) {
  let arguments;
};

g().next();

assert.sameValue(typeof args, 'object');
assert.sameValue(args.length, 0);
