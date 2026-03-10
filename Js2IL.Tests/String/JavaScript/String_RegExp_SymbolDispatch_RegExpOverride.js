"use strict";

var re = /a/g;

re[Symbol.match] = function (input) {
    console.log("match");
    console.log(this === re);
    console.log(input);
    return ["OVERRIDE-MATCH"];
};

re[Symbol.replace] = function (input, replacement) {
    console.log("replace");
    console.log(this === re);
    console.log(input);
    console.log(replacement);
    return "OVERRIDE-REPLACE";
};

re[Symbol.search] = function (input) {
    console.log("search");
    console.log(this === re);
    console.log(input);
    return 42;
};

re[Symbol.split] = function (input, limit) {
    console.log("split");
    console.log(this === re);
    console.log(input);
    console.log(limit);
    return ["OVERRIDE-SPLIT", limit];
};

console.log("aba".match(re)[0]);
console.log("aba".replace(re, "x"));
console.log("aba".search(re));
var splitResult = "aba".split(re, 3);
console.log(splitResult[0]);
console.log(splitResult[1]);
