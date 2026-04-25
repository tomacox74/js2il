"use strict";

let callCount = 0;

const f = (_ = (function () { throw new Error("boom"); }())) => {
    callCount = callCount + 1;
};

try {
    f();
} catch (e) {
    console.log(e.message);
}

console.log(callCount);
