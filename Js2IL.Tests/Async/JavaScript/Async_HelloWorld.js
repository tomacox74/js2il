"use strict";\r\n\r\nasync function helloWorld() {
    console.log("Hello, World!");
}

console.log("Before calling async function");

helloWorld();

console.log("After calling async function");