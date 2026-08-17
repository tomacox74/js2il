"use strict";

// Fixed-length and length-tracking views over a resizable buffer must keep
// tracking the buffer across shrink, out-of-bounds, and regrow. Views over
// non-resizable buffers have an immutable length and take the fast path.
const rab = new ArrayBuffer(16, { maxByteLength: 64 });
const fixedView = new Int32Array(rab, 0, 4);
const trackingView = new Int32Array(rab);

fixedView[0] = 111;
fixedView[3] = 444;
console.log(fixedView.length);
console.log(trackingView.length);
console.log(fixedView[0]);
console.log(fixedView[3]);

// Shrink below the fixed-length view so it becomes out of bounds.
rab.resize(8);
console.log(fixedView.length);
console.log(fixedView.byteLength);
console.log(fixedView.byteOffset);
console.log(trackingView.length);
console.log(trackingView[0]);

// Writes through an out-of-bounds view must be ignored.
fixedView[0] = 999;
console.log(trackingView[0]);

// Regrowing restores the fixed-length view.
rab.resize(64);
console.log(fixedView.length);
console.log(trackingView.length);
console.log(fixedView[0]);
fixedView[3] = 777;
console.log(trackingView[3]);

// A view at a non-zero offset over a resizable buffer.
const offsetView = new Int32Array(rab, 32);
console.log(offsetView.length);
console.log(offsetView.byteOffset);
rab.resize(16);
console.log(offsetView.length);
console.log(offsetView.byteOffset);

// Views over non-resizable buffers never change length.
const fixedBuffer = new ArrayBuffer(8);
const plain = new Int32Array(fixedBuffer);
plain[1] = 42;
plain[-1] = 91;
plain[2147483648] = 92;
plain[1e100] = 93;
console.log(plain.length);
console.log(plain[0]);
console.log(plain[1]);
console.log(new Int32Array(0).length);
