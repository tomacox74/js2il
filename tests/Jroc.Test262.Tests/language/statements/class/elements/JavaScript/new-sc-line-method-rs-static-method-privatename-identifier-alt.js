// Upstream: test/language/statements/class/elements/new-sc-line-method-rs-static-method-privatename-identifier-alt.js
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
