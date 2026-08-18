"use strict";

function sum(a, b, c) { return this.base + a + b + c; }

// apply / call / bind with varying argument counts (exercise both inline and
// array-backed JsCallArguments storage paths).
console.log(sum.apply({ base: 100 }, [1, 2, 3]));
console.log(sum.call({ base: 100 }, 1, 2, 3));
console.log(sum.bind({ base: 100 })(1, 2, 3));
console.log(sum.bind({ base: 100 }, 1, 2)(3));

function variadicSum() {
    let total = this.base;
    for (let i = 0; i < arguments.length; i++) {
        total += arguments[i];
    }
    return total;
}
console.log(variadicSum.call({ base: 0 }, 1, 2, 3, 4, 5, 6, 7));
console.log(variadicSum.apply({ base: 0 }, [1, 2, 3, 4, 5, 6, 7, 8]));

// toString
console.log(Function.prototype.toString.call(sum).indexOf("function") === 0);

// Non-function receivers throw TypeError for apply/call/bind/toString.
try {
    Function.prototype.apply.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    Function.prototype.call.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    Function.prototype.bind.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    Function.prototype.toString.call({});
} catch (error) {
    console.log(error instanceof TypeError);
}

// Restricted `caller`/`arguments` own accessor properties on ordinary functions.
try {
    sum.caller;
} catch (error) {
    console.log(error instanceof TypeError);
}
try {
    sum.arguments;
} catch (error) {
    console.log(error instanceof TypeError);
}

// Metadata.
console.log(Function.prototype.apply.length);
console.log(Function.prototype.call.length);
console.log(Function.prototype.bind.length);
console.log(Function.prototype.toString.length);
