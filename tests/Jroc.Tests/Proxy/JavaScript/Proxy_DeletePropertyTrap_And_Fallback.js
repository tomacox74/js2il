"use strict";

const trappedTarget = { keep: 1, removeMe: 2, blocked: 3 };
const seen = [];

const trappedProxy = new Proxy(trappedTarget, {
  deleteProperty: function (target, prop) {
    seen.push(prop);
    if (prop === "blocked") {
      return false;
    }

    return delete target[prop];
  },
});

console.log(delete trappedProxy.removeMe);
console.log("removeMe" in trappedTarget);
console.log(delete trappedProxy.blocked);
console.log("blocked" in trappedTarget);
console.log(seen.join(","));

const fallbackTarget = { fallback: 1 };
const fallbackProxy = new Proxy(fallbackTarget, {});
console.log(delete fallbackProxy.fallback);
console.log("fallback" in fallbackTarget);
