"use strict";

class VectorMask {
    constructor(size) {
        this.end = size >>> 0;
        this.words = new Int32Array(20);
    }

    apply() {
        for (let index = 1; index < this.end; index++) {
            this.words[index] |= 0x40000005;
        }
    }
}

for (const end of [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 15, 17, 20, 22]) {
    const mask = new VectorMask(end);
    const expected = new Int32Array(20);
    for (let index = 0; index < 20; index++) {
        mask.words[index] = index % 3;
        expected[index] = index % 3;
    }
    for (let index = 1; index < end; index++) {
        expected[index] |= 0x40000005;
    }
    mask.apply();
    let same = true;
    for (let index = 0; index < 20; index++) {
        if (mask.words[index] !== expected[index]) {
            same = false;
        }
    }
    console.log(same);
}

const resizable = new VectorMask(9);
resizable.words = new Int32Array(new ArrayBuffer(80, { maxByteLength: 160 }));
resizable.apply();
let resizableMatches = true;
for (let index = 0; index < 20; index++) {
    if (resizable.words[index] !== (index > 0 && index < 9 ? 0x40000005 : 0)) {
        resizableMatches = false;
    }
}
console.log(resizableMatches);

class OrdinaryArrayMask {
    constructor() {
        this.end = 6;
        this.words = [0, 0, 0, 0, 0, 0];
    }

    apply() {
        for (let index = 1; index < this.end; index++) {
            this.words[index] |= 0x40000005;
        }
    }
}

const ordinary = new OrdinaryArrayMask();
ordinary.apply();
console.log(ordinary.words[0] === 0 && ordinary.words[5] === 0x40000005);
