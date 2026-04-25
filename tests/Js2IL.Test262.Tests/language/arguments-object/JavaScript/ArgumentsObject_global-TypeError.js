"use strict";

const err = new TypeError("boom");

console.log(globalThis.TypeError === TypeError);
console.log(err instanceof TypeError);
console.log(err instanceof Error);
