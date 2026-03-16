"use strict";

const crypto = require("crypto");

const hmac = crypto.createHmac("sha256", "key");
hmac.update("a");
hmac.update(Buffer.from("bc"));
console.log("hmac sha256 hex:", hmac.digest("hex"));

console.log("hmac md5 hex:", crypto.createHmac("md5", "key").update("abc").digest("hex"));
console.log("hmac sha384 hex:", crypto.createHmac("sha384", "key").update("abc").digest("hex"));
console.log("hmac sha512 hex:", crypto.createHmac("sha512", "key").update("abc").digest("hex"));

const hmacBuffer = crypto.createHmac("sha1", Buffer.from("key")).update("abc").digest();
console.log("hmac buffer:", Buffer.isBuffer(hmacBuffer));
console.log("hmac sha1 hex:", hmacBuffer.toString("hex"));
