"use strict";

const proto = { y: 2 };

const o = Object.create(proto, {
  x: { value: 1, enumerable: true },
  hidden: { value: 99, enumerable: false }
});

console.log(o.x);
console.log(o.y);

const keys = [];
for (const k in o) {
  keys.push(k);
}
console.log(keys.join(","));

console.log(Object.getOwnPropertyDescriptor(o, "hidden").enumerable === false);
