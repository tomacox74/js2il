"use strict";

function countArgs() {
    return arguments.length;
}

console.log(countArgs.apply(null, null));
