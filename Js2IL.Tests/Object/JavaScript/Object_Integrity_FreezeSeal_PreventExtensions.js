"use strict";

var o = { a: 1 };
console.log("extensible0=" + Object.isExtensible(o));
Object.preventExtensions(o);
o.b = 2;
console.log("extensible1=" + Object.isExtensible(o));
console.log("has_b=" + Object.hasOwn(o, "b"));

var s = { x: 1 };
Object.seal(s);
console.log("sealed=" + Object.isSealed(s));
s.x = 3;
console.log("sealed_write=" + s.x);
console.log("sealed_delete=" + delete s.x);

var f = { y: 1 };
Object.freeze(f);
f.y = 5;
console.log("frozen=" + Object.isFrozen(f));
console.log("frozen_write=" + f.y);
console.log("frozen_delete=" + delete f.y);
