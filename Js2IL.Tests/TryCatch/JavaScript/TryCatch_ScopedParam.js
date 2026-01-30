"use strict";\r\n\r\nlet x = "outer";

try {
  throw "inner";
} catch (x) {
  console.log(x);
}

console.log(x);
