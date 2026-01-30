"use strict";\r\n\r\ntry {
    console.log("in try");
    throw new Error("boom");
} finally {
    console.log("in finally");
}

console.log("after finally");
