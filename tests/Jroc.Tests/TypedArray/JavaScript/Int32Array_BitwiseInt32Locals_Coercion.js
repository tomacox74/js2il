"use strict";

function storeBit(index) {
    const words = new Int32Array(2);
    const wordOffset = index >>> 5;
    const bitOffset = index & 31;
    const mask = 1 << bitOffset;
    words[wordOffset] = mask;
    return words[0];
}

console.log(storeBit(0));
console.log(storeBit(31));
console.log(storeBit(32));
console.log(storeBit(-1));
console.log(storeBit(NaN));
console.log(storeBit(Infinity));
console.log(storeBit(4294967296));
console.log((-1 >>> 0));

let conversions = 0;
const input = {
    valueOf() {
        conversions++;
        return 31;
    }
};
console.log(storeBit(input));
console.log(conversions);
console.log(typeof (1 << 31));

let shift = 5;
const reduced = -1 >>> shift;
shift = 0;
console.log(reduced);
console.log(-1 >>> shift);
