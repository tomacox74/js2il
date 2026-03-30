"use strict";

const p1 = Promise.reject("Error 1");
const p2 = Promise.resolve("Success");
const p3 = Promise.reject("Error 3");

Promise.any([p1, p2, p3]).then((result) => {
    console.log("First fulfilled:", result);
});
