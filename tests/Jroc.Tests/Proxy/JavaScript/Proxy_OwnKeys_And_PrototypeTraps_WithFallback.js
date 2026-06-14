"use strict";

const ownKeysProxy = new Proxy({ ignored: true }, {
  ownKeys: function () {
    return ["z", "1", "a"];
  },
});

console.log(Object.keys(ownKeysProxy).join(","));

const trappedPrototype = { marker: "trapProto" };
const prototypeTarget = {};
const prototypeProxy = new Proxy(prototypeTarget, {
  getPrototypeOf: function () {
    return trappedPrototype;
  },
  setPrototypeOf: function (target, proto) {
    target.protoMarker = proto.marker;
    return true;
  },
});

console.log(Object.getPrototypeOf(prototypeProxy).marker);
Object.setPrototypeOf(prototypeProxy, { marker: "setTrap" });
console.log(prototypeTarget.protoMarker);

const fallbackTarget = {};
fallbackTarget.b = 2;
fallbackTarget.a = 1;
fallbackTarget[0] = 0;
const fallbackProto = { from: "fallbackProto" };
Object.setPrototypeOf(fallbackTarget, fallbackProto);

const fallbackProxy = new Proxy(fallbackTarget, {});
console.log(Object.keys(fallbackProxy).join(","));
console.log(Object.getPrototypeOf(fallbackProxy) === fallbackProto);
Object.setPrototypeOf(fallbackProxy, null);
console.log(Object.getPrototypeOf(fallbackTarget) === null);
