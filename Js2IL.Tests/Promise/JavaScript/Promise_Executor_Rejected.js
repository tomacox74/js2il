"use strict";

const p = new Promise((resolve, reject) => {
    reject("Error occurred");
});

p.then(null, (message) => {
    console.log("Promise rejected with message:", message);
});