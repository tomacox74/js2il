"use strict";
class BitArray { constructor(size) { this.wordArray = new Int32Array(1 + (size >>> 5)); } }
class PrimeSieve {
  constructor(sieveSize) {
    this.sieveSize = sieveSize;
    this.sieveSizeInBits = sieveSize >>> 1;
    // Only compute cap; don't new BitArray yet
    this.cap = 1 + this.sieveSizeInBits;
  }
}
new PrimeSieve(100000);
console.log('ok');
