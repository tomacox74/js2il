"use strict";

// Proxy get trap overrides property lookup
const target = { a: 1, b: 2 };

const handler = {
  get: function (t, prop, receiver) {
    if (prop === "a") return 42;
    return t[prop];
  },
};

const p = new Proxy(target, handler);
console.log(p.a);
console.log(p.b);
