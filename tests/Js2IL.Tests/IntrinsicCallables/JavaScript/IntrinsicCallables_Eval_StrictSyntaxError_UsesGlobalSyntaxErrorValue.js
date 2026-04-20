"use strict";

try {
    (function fun() {
        eval("arguments = 10");
    })(30);

    console.log("no-error");
} catch (e) {
    console.log(e.name);
    console.log(e.constructor === SyntaxError);
    console.log(String(e).includes("SyntaxError"));
}
