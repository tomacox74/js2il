"use strict";\r\n\r\nconst obj = { a: 1 };
console.log("a" in obj);
console.log("b" in obj);
// numeric key presence (not set) should be false
console.log(100 in obj);
