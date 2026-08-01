"use strict";

var order = [];
var innerReadCount = 0;
var outerReadCount = 0;
var nested = {
    get value() {
        outerReadCount++;
        order.push("outer");
        return 42;
    }
};
var values = [];

Object.defineProperty(values, "0", {
    get() {
        innerReadCount++;
        order.push("inner");
        return nested;
    }
});

console.log(values[0].value);
console.log(order.join(","), innerReadCount, outerReadCount);
