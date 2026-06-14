"use strict";

const err = TypeError("oops");
console.log(err.name + ":" + err.message);
console.log(err instanceof TypeError);
console.log(TypeError.prototype.constructor === TypeError);
console.log(TypeError.prototype.name + ":" + TypeError.prototype.message);

try {
    ({}) instanceof ({});
} catch (e) {
    console.log(e instanceof TypeError);
    console.log(e.name + ":" + e.message);
}
