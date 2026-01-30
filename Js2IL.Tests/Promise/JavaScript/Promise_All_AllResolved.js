"use strict";\r\n\r\nconst p1 = Promise.resolve(1);
const p2 = Promise.resolve(2);
const p3 = Promise.resolve(3);

Promise.all([p1, p2, p3]).then((results) => {
    console.log("All resolved:", results[0], results[1], results[2]);
});
