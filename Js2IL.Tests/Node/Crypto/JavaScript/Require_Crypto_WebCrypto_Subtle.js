"use strict";

const crypto = require("crypto");

async function run() {
    const subtle = crypto.webcrypto.subtle;

    const digest = await subtle.digest("SHA-256", Buffer.from("abc"));
    const digestBytes = new Uint8Array(digest);
    console.log("digest byteLength:", digest.byteLength);
    console.log("digest hex:", Buffer.from(Array.from(digestBytes)).toString("hex"));
    console.log("digest sha1 hex:", Buffer.from(Array.from(new Uint8Array(await subtle.digest("SHA-1", Buffer.from("abc"))))).toString("hex"));
    console.log("digest sha384 hex:", Buffer.from(Array.from(new Uint8Array(await subtle.digest("SHA-384", Buffer.from("abc"))))).toString("hex"));
    console.log("digest sha512 hex:", Buffer.from(Array.from(new Uint8Array(await subtle.digest("SHA-512", Buffer.from("abc"))))).toString("hex"));

    const key = await subtle.importKey(
        "raw",
        Buffer.from("key"),
        { name: "HMAC", hash: "SHA-256", length: 24 },
        false,
        ["sign", "verify"]);

    console.log("key type:", key.type);
    console.log("key algorithm:", key.algorithm.name);
    console.log("key hash:", key.algorithm.hash.name);
    console.log("key extractable:", key.extractable);
    console.log("key usages:", key.usages.join(","));

    const signature = await subtle.sign("HMAC", key, Buffer.from("abc"));
    const signatureBytes = new Uint8Array(signature);
    console.log("signature hex:", Buffer.from(Array.from(signatureBytes)).toString("hex"));
    console.log("verify true:", await subtle.verify("HMAC", key, signature, Buffer.from("abc")));

    const tampered = Buffer.from(Array.from(signatureBytes));
    tampered[0] = (tampered[0] ^ 255) & 255;
    console.log("verify false:", await subtle.verify({ name: "HMAC" }, key, tampered, Buffer.from("abc")));
}

run().then(() => {
    console.log("subtle done");
});
