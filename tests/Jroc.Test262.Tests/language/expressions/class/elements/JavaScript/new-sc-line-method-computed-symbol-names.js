// Upstream: test/language/expressions/class/elements/new-sc-line-method-computed-symbol-names.js
function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  if (actual === undefined) {
    console.log(false);
    return false;
  }

  var ok = true;
  if (Object.prototype.hasOwnProperty.call(desc, 'value')) ok = ok && Object.is(actual.value, desc.value);
  if (Object.prototype.hasOwnProperty.call(desc, 'writable')) ok = ok && Object.is(actual.writable, desc.writable);
  if (Object.prototype.hasOwnProperty.call(desc, 'enumerable')) ok = ok && Object.is(actual.enumerable, desc.enumerable);
  if (Object.prototype.hasOwnProperty.call(desc, 'configurable')) ok = ok && Object.is(actual.configurable, desc.configurable);
  console.log(ok);
  return ok;
}

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
