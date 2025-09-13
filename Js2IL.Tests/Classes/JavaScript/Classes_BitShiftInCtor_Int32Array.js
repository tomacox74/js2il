// Minimal repro for invalid IL in class constructor
// - Uses shift/and arithmetic and Int32Array length expression similar to PrimeSieve/BitArray

class BitBag {
  constructor(n) {
    // allocate Int32Array of size (1 + (n >> 5))
    this.buf = new Int32Array(1 + (n >> 5));
  }
  set(i) {
    const w = i >> 5;
    const b = i & 31;
    this.buf[w] = (1 << b);
  }
  test(i) {
    const w = i >> 5;
    const b = i & 31;
    return (this.buf[w] & (1 << b));
  }
}

// smoke: construct and perform a couple ops
const bag = new BitBag(64);
bag.set(1);
console.log(bag.test(1));
