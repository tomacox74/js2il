"use strict";

const source = new Int32Array([1, 2, 3, 4]);
source.set([9, 8], 2);
for (let i = 0; i < source.length; i++) {
  console.log(source[i]);
}

const overlap = source.subarray(1, 3);
source.set(overlap, 0);
for (let i = 0; i < source.length; i++) {
  console.log(source[i]);
}

try {
  source.set([7, 6, 5], 2);
} catch (e) {
  console.log(e.name);
  console.log(e.message);
}

try {
  source.set([1], -1);
} catch (e) {
  console.log(e.name);
  console.log(e.message);
}
