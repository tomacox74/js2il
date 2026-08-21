"use strict";

let receiver;
receiver = new Int32Array([1, 2, 3]);
console.log(receiver.includes(1) ? "cold included" : "cold missing");

for (let iteration = 0; iteration < 1; iteration++) {
    console.log(receiver.includes(2) ? "included" : "missing");

    receiver.includes = function () {
        return false;
    };
    console.log(receiver.includes(2) ? "own" : "own override");

    delete receiver.includes;
    Object.setPrototypeOf(receiver, {
        includes() {
            return false;
        }
    });
    console.log(receiver.includes(2) ? "custom" : "custom override");

    Object.setPrototypeOf(receiver, Int32Array.prototype);
    Int32Array.prototype.includes = function () {
        return false;
    };
    console.log(receiver.includes(2) ? "prototype" : "prototype override");
}
