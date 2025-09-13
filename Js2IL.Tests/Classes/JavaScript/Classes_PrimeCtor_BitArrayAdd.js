// Minimal repro for invalid IL in class constructor
// Two constructor lines only, per request:
//   this.sieveSizeInBits = sieveSize >>> 1;
//   this.bitArray = new BitArray(1 + this.sieveSizeInBits);

class BitArray {
  constructor(n) {
    // no-op; just hold the size for sanity
    this.n = n;
  }
}

class PrimeSieve {
  constructor(sieveSize) {
    this.sieveSizeInBits = sieveSize >>> 1;
    this.bitArray = new BitArray(1 + this.sieveSizeInBits);
  }
}

// Force constructor execution
new PrimeSieve(100);
