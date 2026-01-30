"use strict";\r\n\r\nsetImmediate(() => console.log("1"));
setImmediate(() => console.log("2"));
setImmediate(() => console.log("3"));
console.log("setImmediate scheduled 3 times.");
