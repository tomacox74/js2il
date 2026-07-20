"use strict";

const int8 = new Int8Array([-129, -128, -127, 127, 128, 129, 255, 256]);
console.log(int8.join("|"));

const int16 = new Int16Array([-32769, -32768, -32767, 32767, 32768, 32769, 65535, 65536]);
console.log(int16.join("|"));

const uint8 = new Uint8Array([-1, 0, 1, 255, 256, 257, 511, 1.9]);
console.log(uint8.join("|"));

const clamped = new Uint8ClampedArray([-20, -0.5, 0.49, 0.5, 1.5, 2.5, 254.6, 255.4, 300, NaN]);
console.log(clamped.join("|"));

let coercions = 0;
const dynamicValue = {
  valueOf() {
    coercions++;
    return 7;
  }
};
const empty = new Uint8Array(0);
empty[0] = dynamicValue;
console.log(coercions);

try {
  empty[0] = Symbol("value");
} catch (error) {
  console.log(error.name);
}
