"use strict";

function vectorOr(size) {
    const end = size >>> 0;
    const words = new Int32Array(end);
    for (let index = 0; index < end; index++) {
        words[index] |= 0x40000005;
    }
    return words;
}

function scalarOr(size) {
    const end = size >>> 0;
    const words = new Int32Array(end);
    for (let index = 0; index < end; index++) {
        words[index] = words[index] | 0x40000005;
    }
    return words;
}

for (const size of [0, 1, 2, 3, 4, 5, 7, 8, 9, 15, 16, 17, 31, 32, 33, 1000]) {
    const actual = vectorOr(size);
    const expected = scalarOr(size);
    let same = actual.length === expected.length;
    for (let index = 0; index < size; index++) {
        if (actual[index] !== expected[index]) {
            same = false;
        }
    }
    console.log(same);
}

function literalBound() {
    const words = new Int32Array(7);
    for (let index = 1; index < 6; index++) {
        words[index] |= 3;
    }
    return words[0] === 0 && words[1] === 3 && words[5] === 3 && words[6] === 0;
}
console.log(literalBound());

function ordinaryArray() {
    const words = [0, 0, 0, 0];
    for (let index = 0; index < 4; index++) {
        words[index] |= 5;
    }
    return words[3] === 5;
}
console.log(ordinaryArray());

function withCallback() {
    const words = new Int32Array(6);
    let visits = 0;
    for (let index = 0; index < 6; index++) {
        words[index] |= 5;
        visits++;
    }
    return visits === 6 && words[5] === 5;
}
console.log(withCallback());

function withDynamicBound() {
    const words = new Int32Array(6);
    let end = 6;
    for (let index = 0; index < end; index++) {
        words[index] |= 5;
    }
    return words[5] === 5;
}
console.log(withDynamicBound());

function withObservableBound() {
    const words = new Int32Array(4);
    let reads = 0;
    function end() {
        reads++;
        return 4;
    }
    for (let index = 0; index < end(); index++) {
        words[index] |= 5;
    }
    return reads === 5 && words[3] === 5;
}
console.log(withObservableBound());

function withAliasedStore() {
    const words = new Int32Array(4);
    const alias = words;
    for (let index = 0; index < 4; index++) {
        words[index] |= 5;
        alias[index] |= 2;
    }
    return words[3] === 7;
}
console.log(withAliasedStore());

function skipUninitializedArray() {
    for (let index = 0; index < 0; index++) {
        words[index] |= 5;
    }
    const words = new Int32Array(1);
    return words[0] === 0;
}
console.log(skipUninitializedArray());

function receiverLength() {
    const words = new Int32Array(5);
    for (let index = 0; index < words.length; index++) {
        words[index] |= 5;
    }
    return words[4] === 5;
}
console.log(receiverLength());
