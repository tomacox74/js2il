"use strict";\r\n\r\nconst logHello = () => {
    console.log("Hello, World!");
};

setImmediate(logHello);
console.log("setImmediate scheduled.");
