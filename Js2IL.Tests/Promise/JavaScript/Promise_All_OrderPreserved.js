"use strict";\r\n\r\n// Promises that resolve in different order should still return results in original order
const p1 = new Promise((resolve) => {
    resolve("first");
});

const p2 = new Promise((resolve) => {
    resolve("second");
});

const p3 = new Promise((resolve) => {
    resolve("third");
});

Promise.all([p1, p2, p3]).then((results) => {
    console.log("Order preserved:", results[0], results[1], results[2]);
});
