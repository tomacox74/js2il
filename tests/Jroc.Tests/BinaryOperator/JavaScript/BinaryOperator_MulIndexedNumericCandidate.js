"use strict";

function multiplyIndexed(left, right) {
    return left[0] * right[0];
}

const events = [];
const leftValue = {
    valueOf() {
        events.push("coerce-left");
        return 6;
    }
};
const left = new Proxy({}, {
    get() {
        events.push("read-left");
        return leftValue;
    }
});
const right = new Proxy({}, {
    get() {
        events.push("read-right");
        return 7;
    }
});

console.log(multiplyIndexed(left, right));
console.log(events.join(","));
console.log(multiplyIndexed(["6"], [7]));
console.log(multiplyIndexed([], [7]));
