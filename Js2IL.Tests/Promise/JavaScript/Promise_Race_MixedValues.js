"use strict";\r\n\r\nconst p1 = Promise.resolve("promise");
const value = "non-promise";

Promise.race([p1, value]).then((result) => {
    console.log("First settled:", result);
});
