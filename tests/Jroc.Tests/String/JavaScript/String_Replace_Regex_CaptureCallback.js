"use strict";

console.log("xaaBAAy".replace(/aa(b)aa/i, function (all, capture, offset, input) {
    console.log(all);
    console.log(capture);
    console.log(offset);
    console.log(input);
    return capture.toUpperCase();
}));

console.log("a-a".replace(/(a)|(b)/g, function (all, a, b) {
    return typeof b;
}));
