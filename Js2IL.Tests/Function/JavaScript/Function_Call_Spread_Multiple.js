"use strict";

function dump() {
    console.log(arguments.length);
    for (let i = 0; i < arguments.length; i++) {
        console.log(arguments[i]);
    }
}

dump(...[1], ...[2, 3], 4);
