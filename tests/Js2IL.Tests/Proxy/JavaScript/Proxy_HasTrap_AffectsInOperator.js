"use strict";

// Proxy has trap affects the `in` operator
const target = { a: 1 };

const handler = {
  has: function (t, prop) {
    if (prop === "b") return true;
    return prop in t;
  },
};

const p = new Proxy(target, handler);
console.log("a" in p);
console.log("b" in p);
console.log("c" in p);
