"use strict";\r\n\r\n// Arrow functions are not hoisted. Declare first, then call.
const helloWorld = () => {
    console.log("Hello, World!", 2);
};

const helloWorldProxy = () => {
    console.log("This is a proxy function.", 1);
    helloWorld();
};

helloWorldProxy();
console.log("finally is now", 3);
