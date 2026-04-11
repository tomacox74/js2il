"use strict";

function f(a, b) {
    console.log(arguments.length);
    console.log(arguments[0]);
    console.log(arguments[1]);
    console.log(arguments[2]);
}

f(1, 2, 3);
f(1);

function outer(a) {
    const arrow = () => {
        console.log(arguments.length);
        console.log(arguments[0]);
    };

    arrow(99);
}

outer(7, 8, 9);

function g() {
    function inner() {
        console.log(arguments.length);
    }

    inner(1, 2);
}

g(1, 2, 3);
