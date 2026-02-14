"use strict";

function dump() {
    console.log(arguments.length);
    for (let i = 0; i < arguments.length; i++) {
        console.log(arguments[i]);
    }
}

function arg(x) {
    console.log("eval" + x);
    return x;
}

function spread() {
    console.log("spread");
    return [2, 3];
}

dump(arg(1), ...spread(), arg(4));
