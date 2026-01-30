"use strict";\r\n\r\n// Minimal reproduction for generator unsupported numeric property keys
const o = {
  10: 1,
  20: 2
};

console.log(o[10]);
