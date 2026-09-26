"use strict";

class Bitset {
    constructor(length) {
        this.words = new Int32Array(length);
    }

    isSet(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        return this.words[wordOffset] & (1 << bitOffset);
    }

    firstZero(index) {
        while (this.isSet(index)) { index++ }
        return index;
    }
}

const bits = new Bitset(3);
bits.words[0] = -1;
bits.words[1] = -1;
bits.words[2] = 2147483647;
for (let start = 0; start < 32; start++) {
    console.log(bits.firstZero(start));
}
console.log(bits.firstZero(32));
console.log(bits.firstZero(64));
console.log(bits.firstZero(96));
bits.words[0] = 2147483647;
bits.words[1] = 11;
console.log(bits.firstZero(0));
console.log(bits.firstZero(32));
console.log(bits.firstZero(33));
console.log(bits.firstZero(35));
console.log(bits.firstZero(-1));
console.log(bits.firstZero(0.5));
