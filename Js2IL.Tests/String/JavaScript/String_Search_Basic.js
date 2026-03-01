"use strict";

const s = "abc123def";
console.log(s.search(/\d+/));
console.log(s.search("123"));
console.log(s.search(/xyz/));
console.log("undefined-value".search());

const globalRe = /a/g;
globalRe.lastIndex = 2;
console.log("baaa".search(globalRe));
console.log(globalRe.lastIndex);
