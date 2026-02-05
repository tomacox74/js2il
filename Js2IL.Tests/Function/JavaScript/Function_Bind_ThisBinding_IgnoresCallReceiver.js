"use strict";

function addBase(x) {
    return this.base + x;
}

const obj = { base: 7 };
const bound = addBase.bind(obj);

const other = { base: 100, m: bound };
console.log(other.m(5));
