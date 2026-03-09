"use strict";

const crypto = require("crypto");

function logError(label, action) {
    try {
        action();
        console.log(label + " threw:", false);
    } catch (error) {
        console.log(label + " threw:", true);
        console.log(label + " name:", error.name);
        console.log(label + " message:", error.message);
    }
}

logError("algorithm type", () => crypto.createHash(123));
logError("algorithm unsupported", () => crypto.createHash("sha3"));
logError("random size type", () => crypto.randomBytes("4"));
logError("random size negative", () => crypto.randomBytes(-1));

const hash = crypto.createHash("sha256");
hash.update("abc");
console.log("digest first:", hash.digest("hex"));
logError("digest twice", () => hash.digest("hex"));
