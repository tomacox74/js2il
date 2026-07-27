const existing = new Map([[1, "existing"]]);
class Callback {}

console.log(existing.getOrInsertComputed(1, Callback));

try {
  new Map().getOrInsertComputed(1, Callback);
} catch (error) {
  console.log(error instanceof TypeError);
}

let sloppyThis;
function sloppyCallback(key) {
  sloppyThis = this;
  return key;
}

const sloppyResult = new Map().getOrInsertComputed(
  "sloppy",
  new Proxy(sloppyCallback, {})
);
console.log(sloppyResult);
console.log(sloppyThis === globalThis);

let strictThis;
function strictCallback(key) {
  "use strict";
  strictThis = this;
  return key;
}

const strictResult = new Map().getOrInsertComputed(
  "strict",
  new Proxy(strictCallback, {})
);
console.log(strictResult);
console.log(strictThis === undefined);

let trappedThis;
const trappedResult = new Map().getOrInsertComputed(
  "trapped",
  new Proxy(sloppyCallback, {
    apply(target, thisArg, args) {
      trappedThis = thisArg;
      return args[0];
    },
  })
);
console.log(trappedResult);
console.log(trappedThis === undefined);
