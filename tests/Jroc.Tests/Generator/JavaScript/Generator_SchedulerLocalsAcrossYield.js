"use strict";

function* test(value) {
    const before = value + 1;
    const resumed = yield before;
    return before + resumed;
}

const iterator = test(4);
console.log(iterator.next().value);
console.log(iterator.next(10).value);
