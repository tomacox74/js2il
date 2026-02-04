"use strict";

function f() {
    const o = { arguments: 123 };
    console.log(o.arguments);
}

f(1, 2, 3);
