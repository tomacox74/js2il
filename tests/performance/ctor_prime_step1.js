"use strict";
class PrimeSieve {
  constructor(sieveSize) {
    this.sieveSize = sieveSize;
    this.sieveSizeInBits = sieveSize >>> 1;
  }
}
new PrimeSieve(100000);
console.log('ok');
