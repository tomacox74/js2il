"use strict";

const url = require("node:url");

console.log(typeof import.meta);
console.log(typeof import.meta.url);
console.log(import.meta === import.meta);
console.log(import.meta.url.startsWith("file://"));
console.log(url.fileURLToPath(import.meta.url) === __filename);
console.log(url.fileURLToPath(new url.URL("./fixtures/demo.txt", import.meta.url)).split("\\").join("/").endsWith("/fixtures/demo.txt"));
