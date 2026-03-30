"use strict";

const p1 = Promise.resolve(1);
const p2 = Promise.reject("Error in p2");
const p3 = Promise.resolve(3);

Promise.all([p1, p2, p3])
    .then((results) => {
        console.log("Should not reach here");
    })
    .catch((error) => {
        console.log("Caught rejection:", error);
    });
