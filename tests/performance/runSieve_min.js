"use strict";
class BitArray { constructor(size) { this.wordArray = new Int32Array(1 + (size >>> 5)); }
  setBitsTrue(a,b,c) {}
  searchBitFalse(x) { return x; }
}
class PrimeSieve {
  constructor(n) { this.sieveSize = n; this.sieveSizeInBits = n >>> 1; this.bitArray = new BitArray(1 + this.sieveSizeInBits); }
  runSieve() {
    const q = Math.ceil(Math.sqrt(this.sieveSizeInBits)); let factor = 1;
    while (factor < q) { const step = factor * 2 + 1; const start = factor * factor * 2 + factor + factor; this.bitArray.setBitsTrue(start, step, this.sieveSizeInBits); factor = this.bitArray.searchBitFalse(factor + 1); }
    return this;
  }
}
new PrimeSieve(100000).runSieve();
console.log('ok');
