"use strict";

const first = /a+/g;
first.lastIndex = 2;
const second = /a+/g;

RegExp = function () {
    throw new Error("literal used the global constructor");
};

const third = /a+/g;
console.log(first !== second);
console.log(second.lastIndex);
console.log(third.test("aa"));
console.log(third.lastIndex);
