"use strict";

function f1(a) {
    console.log("f1", a);
}

function f2(a, b) {
    console.log("f2", a, b);
}

function f3(a, b, c) {
    console.log("f3", a, b, c);
}

function f4(a, b, c, d) {
    console.log("f4", a, b, c, d);
}

function f5(a, b, c, d, e) {
    console.log("f5", a, b, c, d, e);
}

function f6(a, b, c, d, e, f) {
    console.log("f6 " + a + " " + b + " " + c + " " + d + " " + e + " " + f);
}

f1(1);
f2(1, 2);
f3(1, 2, 3);
f4(1, 2, 3, 4);
f5(1, 2, 3, 4, 5);
f6(1, 2, 3, 4, 5, 6);
