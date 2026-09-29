"use strict";

const value = "abcdef";
console.log(value.charAt(1) + ":" + value.charCodeAt(1));
console.log(value.slice(1) + ":" + value.slice(-3, -1));
console.log(value.substr(1) + ":" + value.substr(-3, 2));
console.log(value.substring(1) + ":" + value.substring(4, 1));

for (const index of [NaN, Infinity, -Infinity, -1, 1.9, 1e100]) {
    console.log("[" + value.charAt(index) + "]:" + value.charCodeAt(index));
    console.log("[" + value.slice(index) + "]:[" + value.substr(index)
        + "]:[" + value.substring(index) + "]");
}

const index = { valueOf() { console.log("coerced"); return 2; } };
console.log(value.charAt(index) + ":" + value.slice(index, 4)
    + ":" + value.substring(index, 4));
console.log(value.slice(1, index) + ":" + value.substr(1, index)
    + ":" + value.substring(1, index));
console.log(value.substr(index, 3));
try {
    value.slice(Symbol("index"));
} catch (error) {
    console.log(error.name);
}
