"use strict";

const logHello = () =>{
    console.log("Hello, World!");
}

const timeoutId = setTimeout(logHello, 0);
console.log("setTimeout with zero delay scheduled.");
clearTimeout(timeoutId);
console.log("Timeout cleared.");
