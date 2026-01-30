"use strict";\r\n\r\n// Library module that exports an object with function properties
// This is the exact pattern from issue #156

function foo() {
    return 'ok';
}

function add(a, b) {
    return a + b;
}

function multiply(a, b) {
    return a * b;
}

module.exports = { foo, add, multiply };
