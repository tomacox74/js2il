"use strict";

const result = JSON.parse(
    '{"a":1,"nested":{"b":2},"remove":3,"arr":[4,5]}',
    function (key, value) {
        if (key === "b") {
            console.log(Object.getPrototypeOf(this) === Object.prototype);
        }
        if (key === "0") {
            console.log(Array.isArray(this));
        }
        if (key === "") {
            console.log(Object.getPrototypeOf(this) === Object.prototype);
            console.log(JSON.stringify(Object.keys(this)));
        }
        if (key === "remove") {
            return undefined;
        }
        return typeof value === "number" ? value * 10 : value;
    }
);

console.log(JSON.stringify(result));
console.log(JSON.parse("1", (key, value) => key === "" ? 2 : value));
console.log(JSON.stringify(JSON.parse('{"x":1}', {})));
