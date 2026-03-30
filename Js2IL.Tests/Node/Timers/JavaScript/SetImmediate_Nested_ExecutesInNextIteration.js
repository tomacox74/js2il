"use strict";

setImmediate(() => {
    console.log("outer");
    setImmediate(() => console.log("inner"));
});

console.log("scheduled nested immediates.");
