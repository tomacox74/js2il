"use strict";

console.log(new Uint8Array(true).length);
console.log(new Uint8Array("3").length);

try {
  new Int32Array(-1);
} catch (error) {
  console.log(error.name);
  console.log(error.message);
}

try {
  new Float64Array(Infinity);
} catch (error) {
  console.log(error.name);
  console.log(error.message);
}

const target = new Int32Array(2);

try {
  target.set();
} catch (error) {
  console.log(error.name);
  console.log(error.message);
}

try {
  target.set(null);
} catch (error) {
  console.log(error.name);
  console.log(error.message);
}
