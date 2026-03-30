"use strict";

// Proxy set trap intercepts writes
const target = { a: 1 };
const seen = [];

const handler = {
  set: function (t, prop, value, receiver) {
    seen.push(prop + "=" + value);
    return true;
  },
};

const p = new Proxy(target, handler);
p.a = 5;

console.log(seen[0]);
console.log(target.a);
