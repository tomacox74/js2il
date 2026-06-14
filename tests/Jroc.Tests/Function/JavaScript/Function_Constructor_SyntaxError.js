"use strict";

try {
    new Function("if (");
    console.log("no-error");
} catch (e) {
    const text = String(e);
    console.log(text.includes("SyntaxError"));
    console.log(text.length > 0);
}
