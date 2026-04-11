"use strict";

var moduleValue = 10;

function outer() {
    let localValue = 20;
    globalThis.dynamicFnVisible = 7;

    const f = new Function("return typeof moduleValue + ',' + typeof localValue + ',' + globalThis.dynamicFnVisible;");
    console.log(f());
}

outer();
