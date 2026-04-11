"use strict";

const buffer = new ArrayBuffer(16);

try {
  new Int32Array(buffer, 2);
} catch (e) {
  console.log(e.name);
  console.log(e.message);
}

try {
  new Float64Array(buffer, 4);
} catch (e) {
  console.log(e.name);
  console.log(e.message);
}
