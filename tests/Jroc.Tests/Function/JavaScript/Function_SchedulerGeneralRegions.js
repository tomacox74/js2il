"use strict";

function mixed(a, b, c, d) {
    return Math.max((a + 1) * (b - 2), (c + 3) / (d + 4));
}

function nested(a, b) {
    return {
        x: [a * 2, b + 3],
        y: Math.floor(a) + Math.sqrt(b)
    };
}

let state = 0;
let order = "";

function read() {
    order += "r";
    return state;
}

function write() {
    order += "w";
    state++;
    return state;
}

function ordered() {
    state = 0;
    order = "";
    const result = read() + write() + read();
    return result + "," + order;
}

function getter(value) {
    order = "";
    const object = {
        _x: value,
        get x() {
            order += "g";
            return this._x;
        },
        set x(next) {
            order += "s";
            this._x = next;
        }
    };
    const result = object.x + (object.x = 2) + object.x;
    return result + "," + order;
}

function caught() {
    state = 0;
    order = "";
    try {
        return (read() + write()) + "," + order;
    } catch (error) {
        return "-1," + order;
    }
}

console.log(mixed(2, 4, 6, 5));
const nestedResult = nested(2, 4);
console.log(nestedResult.x.join(",") + ":" + nestedResult.y);
console.log(ordered());
console.log(getter(1));
console.log(caught());
