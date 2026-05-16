"use strict";

try {
    new Enumerator(123);
    console.log("unreachable");
} catch (e) {
    console.log("caught");
}

console.log("done");
