"use strict";

const globalA = Symbol.for("registry-key");
const globalB = Symbol.for("registry-key");
const local = Symbol("registry-key");

console.log(globalA === globalB);
console.log(globalA === local);
console.log(Symbol.keyFor(globalA));
console.log(Symbol.keyFor(local) == null);

console.log(typeof Symbol.iterator);
console.log(Symbol.iterator === Symbol.iterator);
console.log(Symbol.toPrimitive === Symbol.toPrimitive);

try {
  Symbol.keyFor("not-a-symbol");
} catch (e) {
  console.log(e.name);
}
