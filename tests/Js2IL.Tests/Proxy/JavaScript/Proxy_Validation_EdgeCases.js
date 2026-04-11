"use strict";

function log(label, fn) {
  try {
    const value = fn();
    console.log(label + ":ok:" + value);
  } catch (error) {
    console.log(label + ":" + error.name);
  }
}

log("proxy-target", function () {
  return new Proxy(1, {});
});

log("proxy-handler", function () {
  return new Proxy({}, 1);
});

log("revocable-target", function () {
  return Proxy.revocable(1, {});
});

log("revocable-handler", function () {
  return Proxy.revocable({}, 1);
});

const applyProxy = new Proxy({}, {
  apply: function () {
    return "applied";
  },
});

log("apply-noncallable", function () {
  return applyProxy();
});

const constructProxy = new Proxy({}, {
  construct: function () {
    return { ok: true };
  },
});

log("construct-nonconstructible", function () {
  return new constructProxy();
});

const badConstructResultProxy = new Proxy(function Base() {}, {
  construct: function () {
    return 1;
  },
});

log("construct-primitive-result", function () {
  return new badConstructResultProxy();
});

const nullPrototypeProxy = new Proxy({}, {
  getPrototypeOf: function () {
    return null;
  },
});

console.log(Object.getPrototypeOf(nullPrototypeProxy) === null);

const badGetPrototypeProxy = new Proxy({}, {
  getPrototypeOf: function () {
    return 1;
  },
});

log("getPrototypeOf-primitive", function () {
  return Object.getPrototypeOf(badGetPrototypeProxy);
});

const arrayLikeOwnKeysProxy = new Proxy({}, {
  ownKeys: function () {
    return { 0: "z", 1: "a", length: 2 };
  },
});

console.log(Object.keys(arrayLikeOwnKeysProxy).join(","));

const symbolOwnKeysProxy = new Proxy({}, {
  ownKeys: function () {
    return { 0: "name", 1: Symbol.iterator, length: 2 };
  },
});

console.log(Object.keys(symbolOwnKeysProxy).join(","));

const badOwnKeysProxy = new Proxy({}, {
  ownKeys: function () {
    return "ab";
  },
});

log("ownKeys-primitive", function () {
  return Object.keys(badOwnKeysProxy).join(",");
});

const badOwnKeysEntryProxy = new Proxy({}, {
  ownKeys: function () {
    return [1];
  },
});

log("ownKeys-entry", function () {
  return Object.keys(badOwnKeysEntryProxy).join(",");
});
