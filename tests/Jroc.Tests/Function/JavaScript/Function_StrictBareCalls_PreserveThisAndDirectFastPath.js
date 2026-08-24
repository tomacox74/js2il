"use strict";

function increment(value) {
    return value + 1;
}

function directThis() {
    return this;
}

function nestedArrowThis() {
    return (() => this)();
}

console.log(increment(41));
console.log(directThis() === undefined);
console.log(nestedArrowThis() === undefined);
