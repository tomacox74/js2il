"use strict";

function f() {
    // Computed property key is an expression, so `arguments` must behave as an implicit binding.
    // If `arguments` is treated as a free/global name, this will throw.
    const o = { [arguments.length]: 123 };
    console.log(o[3]);
}

f(1, 2, 3);
