"use strict";

function callDynamic(body) {
    return Function(body);
}

function constructDynamic(body) {
    return new Function(body);
}

try {
    callDynamic("return 1;");
    console.log("call-no-error");
} catch (e) {
    console.log(String(e).includes("statically discoverable source strings"));
}

try {
    constructDynamic("return 1;");
    console.log("new-no-error");
} catch (e) {
    console.log(String(e).includes("statically discoverable source strings"));
}
