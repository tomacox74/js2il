"use strict";

var length = 6;
var seen = [];
var i;

for (i = 0; i < (length - 2); i += 3) {
    seen.push(i);
}

console.log(seen.join(","));

seen = [];
i = 0;
while (i < (length - 2)) {
    seen.push(i);
    i += 3;
}

console.log(seen.join(","));
