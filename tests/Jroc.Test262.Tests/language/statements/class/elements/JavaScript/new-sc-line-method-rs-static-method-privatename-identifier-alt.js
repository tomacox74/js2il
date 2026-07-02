// Upstream: test/language/statements/class/elements/new-sc-line-method-rs-static-method-privatename-identifier-alt.js
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

class C {
  static #$(value) { return value; }
  static #_(value) { return value; }
  static #o(value) { return value; };
  m() { return 42; }
  static $(value) { return this.#$(value); }
  static _(value) { return this.#_(value); }
  static o(value) { return this.#o(value); }
}

var c = new C();
assert.sameValue(c.m(), 42);
assert.sameValue(c.m, C.prototype.m);
assert(!Object.prototype.hasOwnProperty.call(c, "m"));
verifyProperty(C.prototype, "m", { enumerable: false, configurable: true, writable: true });
assert.sameValue(C.$(1), 1);
assert.sameValue(C._(1), 1);
assert.sameValue(C.o(1), 1);
