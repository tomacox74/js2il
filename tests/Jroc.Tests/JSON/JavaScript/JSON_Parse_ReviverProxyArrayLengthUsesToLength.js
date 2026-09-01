"use strict";

const sentinel = new Error("index getter reached");
const replacement = new Proxy([], {
  get: function (target, property) {
    if (property === "length") {
      return Infinity;
    }
    if (property === "0") {
      throw sentinel;
    }
    return target[property];
  },
});

try {
  JSON.parse("[null,null]", function (name, value) {
    if (name === "0") {
      this[1] = replacement;
    }
    return value;
  });
} catch (error) {
  console.log(error === sentinel);
}
