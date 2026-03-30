"use strict";

const body = "return 1;";

try {
    Function(body);
    console.log("call-no-error");
} catch (e) {
    console.log(String(e).includes("string literal arguments"));
}

try {
    new Function(body);
    console.log("new-no-error");
} catch (e) {
    console.log(String(e).includes("string literal arguments"));
}
