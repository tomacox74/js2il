"use strict";

try {
    console.log("in try");
    throw new Error("boom");
} finally {
    console.log("in finally");
}

console.log("after finally");
