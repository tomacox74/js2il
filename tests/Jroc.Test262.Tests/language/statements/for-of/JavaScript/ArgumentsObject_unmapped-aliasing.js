"use strict";

(function (a, b, c) {
    const seen = [];

    for (var value of arguments) {
        a = b;
        b = c;
        c = seen.length;
        seen.push(value);
    }

    console.log(seen.join(","));
    console.log(arguments[0]);
    console.log(arguments[1]);
    console.log(arguments[2]);
}(1, 2, 3));
