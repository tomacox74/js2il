"use strict";

function formatSum(a, b, enabled, prefix) {
    if (enabled) {
        return prefix + (a + b);
    }

    return "off";
}

console.log(formatSum(1, 2, true, "sum="));
console.log(formatSum(4, 5, true, "sum="));
