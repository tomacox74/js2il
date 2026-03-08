"use strict";

const custom = {};

custom[Symbol.match] = function (input) {
    console.log(this === custom);
    console.log(input);
    return ["MATCH"];
};

custom[Symbol.replace] = function (input, replacement) {
    console.log(this === custom);
    console.log(input);
    console.log(replacement);
    return "R";
};

custom[Symbol.search] = function (input) {
    console.log(this === custom);
    console.log(input);
    return 7;
};

custom[Symbol.split] = function (input, limit) {
    console.log(this === custom);
    console.log(input);
    console.log(limit);
    return ["S1", "S2"];
};

var match = "abc".match(custom);
console.log(match[0]);
console.log("abc".replace(custom, "x"));
console.log("abc".search(custom));

var split = "abc".split(custom, 5);
console.log(split.length);
console.log(split[0]);
console.log(split[1]);
