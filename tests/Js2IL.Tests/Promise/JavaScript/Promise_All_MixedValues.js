"use strict";

const p1 = Promise.resolve("promise");
const value = "non-promise";
const num = 42;

Promise.all([p1, value, num]).then((results) => {
    console.log("Mixed results:", results[0], results[1], results[2]);
});
