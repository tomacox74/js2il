"use strict";\r\n\r\n// javascript allows functions to be hoisted.. 
// meaning functions like hellowWorldProxy can reference helloWorld before it has been declared
// this also tests that the function has access to the global closure
helloWorldProxy();

function helloWorldProxy() {
    console.log("This is a proxy function.", 1); 
    helloWorld();
};

function helloWorld() {
    console.log("Hello, World!", 2);
}

console.log("finally is now", 3);
