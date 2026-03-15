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
logError("hmac algorithm type", () => crypto.createHmac(123, "key"));
logError("hmac algorithm unsupported", () => crypto.createHmac("sha3", "key"));
logError("random size type", () => crypto.randomBytes("4"));
logError("random size negative", () => crypto.randomBytes(-1));

const hash = crypto.createHash("sha256");
hash.update("abc");
console.log("digest first:", hash.digest("hex"));
logError("digest twice", () => hash.digest("hex"));

async function logPromiseError(label, action) {
    try {
        await action();
        console.log(label + " threw:", false);
    } catch (error) {
        console.log(label + " threw:", true);
        console.log(label + " name:", error.name);
        console.log(label + " message:", error.message);
    }
}

async function runWebCryptoErrors() {
    const subtle = crypto.webcrypto.subtle;

    await logPromiseError("subtle digest unsupported", () => subtle.digest("MD5", Buffer.from("abc")));
    await logPromiseError("subtle import format", () => subtle.importKey(
        "jwk",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["sign"]));
    await logPromiseError("subtle import key data", () => subtle.importKey(
        "raw",
        "key",
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["sign"]));

    const verifyOnlyKey = await subtle.importKey(
        "raw",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["verify"]);

    await logPromiseError("subtle sign algorithm", () => subtle.sign("AES-GCM", verifyOnlyKey, Buffer.from("abc")));
    await logPromiseError("subtle sign usage", () => subtle.sign("HMAC", verifyOnlyKey, Buffer.from("abc")));
}

runWebCryptoErrors().then(() => {
    console.log("error paths done");
});
