"use strict";\r\n\r\nvar g = "Hello";

const outer = (p) => {
    const inner = () => {
        console.log("param:", p);
        console.log("global:", g);
    };
    return inner;
};

var f = outer(123);
f();
