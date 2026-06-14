"use strict";

const url = require("url");

const fileUrl = url.pathToFileURL("folder with space/demo.txt");
console.log("protocol:", fileUrl.protocol);
console.log("href file:", fileUrl.href.startsWith("file://"));
console.log("pathname escaped:", fileUrl.pathname.indexOf("folder%20with%20space") >= 0);
console.log("roundtrip contains:", url.fileURLToPath(fileUrl).indexOf("folder with space") >= 0);

const fromString = url.fileURLToPath("file:///var/example.txt");
console.log("from string suffix:", fromString.indexOf("example.txt") >= 0);
