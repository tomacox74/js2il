// Upstream: test/language/expressions/class/elements/new-sc-line-method-computed-symbol-names.js
var x = Symbol();
var y = Symbol();

var C = class {
  [x]; [y] = 42;
  m() { return 42; }
};

var c = new C();

assert.sameValue(c.m(), 42);
assert.sameValue(c.m, C.prototype.m);
assert(!Object.prototype.hasOwnProperty.call(c, "m"));
verifyProperty(C.prototype, "m", { enumerable: false, configurable: true, writable: true });
assert(!Object.prototype.hasOwnProperty.call(C.prototype, x));
assert(!Object.prototype.hasOwnProperty.call(C, x));
verifyProperty(c, x, { value: undefined, enumerable: true, writable: true, configurable: true });
assert(!Object.prototype.hasOwnProperty.call(C.prototype, y));
assert(!Object.prototype.hasOwnProperty.call(C, y));
verifyProperty(c, y, { value: 42, enumerable: true, writable: true, configurable: true });
assert(!Object.prototype.hasOwnProperty.call(C.prototype, "x"));
assert(!Object.prototype.hasOwnProperty.call(C, "x"));
assert(!Object.prototype.hasOwnProperty.call(c, "x"));
assert(!Object.prototype.hasOwnProperty.call(C.prototype, "y"));
assert(!Object.prototype.hasOwnProperty.call(C, "y"));
assert(!Object.prototype.hasOwnProperty.call(c, "y"));
