"use strict";\r\n\r\nconst p = new Promise((resolve) => {
    resolve("Hello World");
});

p.then((message) => {
    console.log("Promise resolved with message:", message);
});