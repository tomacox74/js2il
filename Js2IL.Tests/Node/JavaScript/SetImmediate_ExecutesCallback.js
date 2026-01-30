"use strict";

const logHello = () => {
    console.log("Hello, World!");
};

setImmediate(logHello);
console.log("setImmediate scheduled.");
