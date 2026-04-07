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
logError("hmac key null", () => crypto.createHmac("sha256", null));
logError("hmac key number", () => crypto.createHmac("sha256", 123));
logError("pbkdf2 digest missing", () => crypto.pbkdf2Sync("password", "salt", 1, 16));
logError("pbkdf2 digest unsupported", () => crypto.pbkdf2Sync("password", "salt", 1, 16, "sha3"));
logError("pbkdf2 password number", () => crypto.pbkdf2Sync(123, "salt", 1, 16, "sha256"));
logError("pbkdf2 salt null", () => crypto.pbkdf2Sync("password", null, 1, 16, "sha256"));
logError("pbkdf2 iterations type", () => crypto.pbkdf2Sync("password", "salt", "1", 16, "sha256"));
logError("pbkdf2 iterations zero", () => crypto.pbkdf2Sync("password", "salt", 0, 16, "sha256"));
logError("pbkdf2 keylen negative", () => crypto.pbkdf2Sync("password", "salt", 1, -1, "sha256"));
logError("random size type", () => crypto.randomBytes("4"));
logError("random size negative", () => crypto.randomBytes(-1));

const hash = crypto.createHash("sha256");
hash.update("abc");
console.log("digest first:", hash.digest("hex"));
logError("digest twice", () => hash.digest("hex"));

const hmac = crypto.createHmac("sha256", "key");
logError("hmac update type", () => hmac.update(123));

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
    await logPromiseError("subtle import empty usages", () => subtle.importKey(
        "raw",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256" },
        false,
        []));
    await logPromiseError("subtle import invalid usage enum", () => subtle.importKey(
        "raw",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["SIGN"]));
    await logPromiseError("subtle import unsupported usage", () => subtle.importKey(
        "raw",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["encrypt"]));
    await logPromiseError("subtle import zero key", () => subtle.importKey(
        "raw",
        Buffer.alloc(0),
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["sign"]));
    await logPromiseError("subtle import length mismatch", () => subtle.importKey(
        "raw",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256", length: 128 },
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
