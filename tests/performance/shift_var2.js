"use strict";
let sieveSize = 1000000;
let sieveSizeInBits = sieveSize >>> 1;
console.log('sieveSizeInBits', sieveSizeInBits);
let y = sieveSizeInBits >>> 5;
console.log('y', y);
let len = 1 + y;
console.log('calc len', len);
