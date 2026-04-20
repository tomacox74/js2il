"use strict";

const arrowFn = () => {};

console.log(arrowFn.hasOwnProperty("caller"));
console.log(arrowFn.hasOwnProperty("arguments"));

function logTypeError(action) {
    try {
        action();
        console.log(false);
    } catch (e) {
        console.log(e.constructor === TypeError);
    }
}

logTypeError(() => arrowFn.caller);
logTypeError(() => { arrowFn.caller = {}; });
logTypeError(() => arrowFn.arguments);
logTypeError(() => { arrowFn.arguments = {}; });
