"use strict";

const crypto = require("crypto");

const hash = crypto.createHash("sha256");
hash.update("a");
hash.update("bc");
console.log("sha256 hex:", hash.digest("hex"));

const digestBuffer = crypto.createHash("sha256").update("abc").digest();
console.log("digest buffer:", Buffer.isBuffer(digestBuffer));
console.log("digest buffer hex:", digestBuffer.toString("hex"));

const random = crypto.randomBytes(8);
console.log("random buffer:", Buffer.isBuffer(random));
console.log("random length:", random.length);
console.log("random hex length:", random.toString("hex").length);
