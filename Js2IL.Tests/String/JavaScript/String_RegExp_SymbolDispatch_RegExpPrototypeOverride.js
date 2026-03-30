"use strict";

var re = /a/g;
var proto = Object.getPrototypeOf(re);
var originalMatch = proto[Symbol.match];
var originalReplace = proto[Symbol.replace];
var originalSearch = proto[Symbol.search];
var originalSplit = proto[Symbol.split];

proto[Symbol.match] = function (input) {
    console.log("proto-match");
    console.log(this === re);
    console.log(input);
    return ["PROTO-MATCH"];
};

proto[Symbol.replace] = function (input, replacement) {
    console.log("proto-replace");
    console.log(this === re);
    console.log(input);
    console.log(replacement);
    return "PROTO-REPLACE";
};

proto[Symbol.search] = function (input) {
    console.log("proto-search");
    console.log(this === re);
    console.log(input);
    return 24;
};

proto[Symbol.split] = function (input, limit) {
    console.log("proto-split");
    console.log(this === re);
    console.log(input);
    console.log(limit);
    return ["PROTO-SPLIT", limit];
};

console.log("aba".match(re)[0]);
console.log("aba".replace(re, "x"));
console.log("aba".search(re));
var splitResult = "aba".split(re, 3);
console.log(splitResult[0]);
console.log(splitResult[1]);

proto[Symbol.match] = originalMatch;
proto[Symbol.replace] = originalReplace;
proto[Symbol.search] = originalSearch;
proto[Symbol.split] = originalSplit;
