"use strict";\r\n\r\nconst p1 = Promise.resolve("promise");
const value = "non-promise";
const num = 42;

Promise.allSettled([p1, value, num]).then((results) => {
    console.log("Result 0 status:", results[0].status, "value:", results[0].value);
    console.log("Result 1 status:", results[1].status, "value:", results[1].value);
    console.log("Result 2 status:", results[2].status, "value:", results[2].value);
});
