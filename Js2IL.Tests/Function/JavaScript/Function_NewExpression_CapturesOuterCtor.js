"use strict";

// Regression: a constructor referenced only via `new` inside a nested function
// must be treated as a free var and captured from the outer scope.

function outer() {
    var Ctor = function () {
        this.value = 123;
    };

    function inner() {
        var obj = new Ctor();
        return obj.value;
    }

    return inner();
}

console.log(outer());
