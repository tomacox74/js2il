"use strict";

const original = Symbol.prototype[Symbol.toPrimitive];
Object.defineProperty(Symbol.prototype, Symbol.toPrimitive, {
  configurable: true,
  value: function () {
    return "null";
  },
});

try {
  JSON.parse(Symbol("value"));
  console.log(false);
} catch (error) {
  console.log(error instanceof TypeError);
}

Object.defineProperty(Symbol.prototype, Symbol.toPrimitive, {
  configurable: true,
  value: original,
});
