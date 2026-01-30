"use strict";\r\n\r\nsetImmediate(() => {
    console.log("outer");
    setImmediate(() => console.log("inner"));
});

console.log("scheduled nested immediates.");
