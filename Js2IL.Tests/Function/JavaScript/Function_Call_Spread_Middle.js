"use strict";

function dump() {
    console.log(arguments.length);
    for (let i = 0; i < arguments.length; i++) {
        console.log(arguments[i]);
    }
}

dump(0, ...[1, 2], 3);
