"use strict";

var o = { a: 1 };
console.log("extensible0=" + Object.isExtensible(o));
Object.preventExtensions(o);
try {
  o.b = 2;
  console.log("prevent_add=ok");
} catch (e) {
  console.log("prevent_add=" + e.name);
}
console.log("extensible1=" + Object.isExtensible(o));
console.log("has_b=" + Object.hasOwn(o, "b"));
console.log("empty_sealed=" + Object.isSealed(Object.preventExtensions({})));
console.log("empty_frozen=" + Object.isFrozen(Object.preventExtensions({})));

var s = { x: 1 };
Object.seal(s);
console.log("sealed=" + Object.isSealed(s));
s.x = 3;
console.log("sealed_write=" + s.x);
console.log("sealed_frozen=" + Object.isFrozen(s));
try {
  delete s.x;
  console.log("sealed_delete=ok");
} catch (e) {
  console.log("sealed_delete=" + e.name);
}
console.log("sealed_has_x=" + Object.hasOwn(s, "x"));

var f = { y: 1 };
Object.freeze(f);
console.log("frozen=" + Object.isFrozen(f));
console.log("frozen_sealed=" + Object.isSealed(f));
try {
  f.y = 5;
  console.log("frozen_write=ok");
} catch (e) {
  console.log("frozen_write=" + e.name);
}
console.log("frozen_value=" + f.y);
try {
  delete f.y;
  console.log("frozen_delete=ok");
} catch (e) {
  console.log("frozen_delete=" + e.name);
}
console.log("frozen_has_y=" + Object.hasOwn(f, "y"));
