"use strict";

function addBase(x) {
    return this.base + x;
}

const obj = { base: 10 };
console.log(addBase.apply(obj, [5]));
