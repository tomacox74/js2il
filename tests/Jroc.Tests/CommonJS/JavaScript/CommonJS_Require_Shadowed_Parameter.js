"use strict";

function test(require) {
    return require('fs');
}

console.log(test((id) => 'shadow:' + id));
