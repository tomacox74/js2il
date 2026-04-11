"use strict";

function add(a, b) {
    return a + b;
}

const bound = add.bind(null, 1);

if (typeof bound === "function") {
    console.log("bound function");
}

if (typeof require === "function") {
    console.log("require function");
}

if (typeof module.require === "function") {
    console.log("module.require function");
}

if (typeof Function.prototype.bind === "function") {
    console.log("prototype bind function");
}
