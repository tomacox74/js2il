"use strict";

function logArgs(...args) {
    console.log("length:", args.length);
    if (args.length > 0) {
        console.log("first:", args[0]);
    }
}

logArgs();
logArgs("hello");
logArgs("a", "b", "c");
