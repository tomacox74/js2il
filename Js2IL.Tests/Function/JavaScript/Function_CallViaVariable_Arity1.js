"use strict";

function addPrefix(x) {
  return "Value: " + x;
}

let f = addPrefix;
console.log(f(42));
