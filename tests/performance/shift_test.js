"use strict";
let sieveSize = 1000000;
let sieveSizeInBits = sieveSize >>> 1;
console.log('sieveSizeInBits', sieveSizeInBits);
let len = 1 + (sieveSizeInBits >>> 5);
console.log('calc len', len);
let a = new Int32Array(len);
console.log('typed length', a.length);
