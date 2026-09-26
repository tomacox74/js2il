"use strict";

class BitArray {
    constructor(size) {
        this.wordArray = new Int32Array(1 + (size >>> 5));
    }

    testBitTrue(index) {
        const wordOffset = index >>> 5;
        const bitOffset = index & 31;
        return this.wordArray[wordOffset] & (1 << bitOffset);
    }
}

class Counter {
    constructor(size) {
        this.sieveSizeInBits = size >>> 1;
        this.bitArray = new BitArray(1 + this.sieveSizeInBits);
    }

    countPrimes() {
        let total = 1;
        for (let index = 1; index < this.sieveSizeInBits; index++) {
            if (!this.bitArray.testBitTrue(index)) {
                total++;
            }
        }
        return total;
    }
}

for (const size of [0, 1, 2, 10, 31, 32, 33, 63, 64, 65, 100, 1000, 10000, 100000, 1000000]) {
    const counter = new Counter(size * 2);
    for (let word = 0; word < counter.bitArray.wordArray.length; word++) {
        counter.bitArray.wordArray[word] = word % 3 === 0 ? -1 : word % 3 === 1 ? 0 : 0x7fffffff;
    }

    let scalar = 1;
    for (let index = 1; index < counter.sieveSizeInBits; index++) {
        if (!counter.bitArray.testBitTrue(index)) {
            scalar++;
        }
    }
    console.log(counter.countPrimes() === scalar);
}
