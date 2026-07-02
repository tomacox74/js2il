// Upstream: test/language/statements/class/elements/after-same-line-static-gen-computed-names.js
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

var x = "b";

class C {
  static *m() { return 42; } [x] = 42; [10] = "meep"; ["not initialized"];
}

var c = new C();
assert.sameValue(C.m().next().value, 42);
assert(!Object.prototype.hasOwnProperty.call(c, "m"));
assert(!Object.prototype.hasOwnProperty.call(C.prototype, "m"));
verifyProperty(C, "m", { enumerable: false, configurable: true, writable: true });
assert(!Object.prototype.hasOwnProperty.call(C.prototype, "b"));
assert(!Object.prototype.hasOwnProperty.call(C, "b"));
verifyProperty(c, "b", { value: 42, enumerable: true, writable: true, configurable: true });
assert(!Object.prototype.hasOwnProperty.call(C.prototype, "10"));
assert(!Object.prototype.hasOwnProperty.call(C, "10"));
verifyProperty(c, "10", { value: "meep", enumerable: true, writable: true, configurable: true });
assert(!Object.prototype.hasOwnProperty.call(C.prototype, "not initialized"));
assert(!Object.prototype.hasOwnProperty.call(C, "not initialized"));
verifyProperty(c, "not initialized", { value: undefined, enumerable: true, writable: true, configurable: true });
