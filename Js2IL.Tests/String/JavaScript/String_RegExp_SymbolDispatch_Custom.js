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
    return 17;
};

custom[Symbol.search] = function (input) {
    console.log(this === custom);
    console.log(input);
    return "SEARCH";
};

custom[Symbol.split] = function (input, limit) {
    console.log(this === custom);
    console.log(input);
    console.log(limit);
    return { marker: "split-result" };
};

var match = "abc".match(custom);
console.log(match[0]);
var replaced = "abc".replace(custom, "x");
console.log(typeof replaced);
console.log(replaced);

var searched = "abc".search(custom);
console.log(typeof searched);
console.log(searched);

var split = "abc".split(custom, 5);
console.log(typeof split);
console.log(split.marker);

var invalid = {};
invalid[Symbol.match] = 1;

try {
    "abc".match(invalid);
} catch (e) {
    console.log(e.name);
    console.log(e.message);
}
