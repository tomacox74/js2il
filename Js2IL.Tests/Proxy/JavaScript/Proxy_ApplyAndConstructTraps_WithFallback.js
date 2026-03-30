"use strict";

function add(a, b) {
  return a + b;
}

const applySeen = [];
const applyProxy = new Proxy(add, {
  apply: function (target, thisArg, args) {
    applySeen.push(args[0]);
    applySeen.push(args[1]);
    return target.apply(thisArg, args) * 2;
  },
});

console.log(applyProxy(2, 3));
console.log(applySeen.join("|"));

function Box(value) {
  this.value = value;
}

let constructProxy;
constructProxy = new Proxy(Box, {
  construct: function (target, args, newTarget) {
    return {
      value: args[0],
      sameNewTarget: newTarget === constructProxy,
    };
  },
});

const trappedInstance = new constructProxy(7);
console.log(trappedInstance.value);
console.log(trappedInstance.sameNewTarget);

const fallbackApplyProxy = new Proxy(add, {});
console.log(fallbackApplyProxy(4, 5));

const fallbackConstructProxy = new Proxy(Box, {});
const fallbackInstance = new fallbackConstructProxy(9);
console.log(fallbackInstance.value);
console.log(Object.getPrototypeOf(fallbackInstance) === Box.prototype);
