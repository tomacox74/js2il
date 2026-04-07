"use strict";

const crypto = require("crypto");

console.log("pbkdf2 sha256 hex:", crypto.pbkdf2Sync("password", "salt", 1, 32, "sha256").toString("hex"));
console.log("pbkdf2 sha1 hex:", crypto.pbkdf2Sync("password", "salt", 2, 20, "sha1").toString("hex"));
console.log("pbkdf2 sha384 hex:", crypto.pbkdf2Sync("password", "salt", 2, 24, "sha384").toString("hex"));
console.log("pbkdf2 sha512 hex:", crypto.pbkdf2Sync(Buffer.from([1, 2, 3]), new Uint8Array([4, 5]), 3, 16, "sha512").toString("hex"));
console.log("pbkdf2 empty length:", crypto.pbkdf2Sync("password", Buffer.from("salt"), 1, 0, "sha256").length);
