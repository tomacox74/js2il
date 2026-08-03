"use strict";

const path = require("path");
const originalJoin = path.join;
const originalSep = path.sep;

function normalize(value) {
    return path.normalize(value);
}

console.log(normalize(123));

path.join = (...parts) => `patched:${parts.join("|")}`;
path.sep = "!";

console.log(path.join("a", "b"));
console.log(path.sep);

path.join = 0;
try {
    path.join("a");
} catch (error) {
    console.log(error.name);
}

delete path.join;
delete path.sep;
try {
    path.join("a");
} catch (error) {
    console.log(error.name);
}
console.log(path.sep === undefined);

path.join = originalJoin;
path.sep = originalSep;
