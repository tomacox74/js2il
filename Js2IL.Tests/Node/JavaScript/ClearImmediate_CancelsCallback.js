"use strict";\r\n\r\nconst handle = setImmediate(() => console.log("should not run"));
console.log("setImmediate scheduled.");
clearImmediate(handle);
console.log("clearImmediate called.");
