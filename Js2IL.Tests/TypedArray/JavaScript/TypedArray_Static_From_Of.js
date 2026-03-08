"use strict";

const mapped = Int32Array.from([1.2, 2.8, -3.1], function (value, index) {
  return value + index + this.bump;
}, { bump: 10 });
console.log(mapped.join("|"));

const arrayLike = Int32Array.from({ 0: 7, 1: 8, length: 2 });
console.log(arrayLike.join("|"));

const uints = Uint8Array.of(257, -1, 3);
console.log(uints.join("|"));

const floats = Float64Array.from(Uint8Array.of(4, 5), function (value) {
  return value / 2;
});
console.log(floats.join("|"));
