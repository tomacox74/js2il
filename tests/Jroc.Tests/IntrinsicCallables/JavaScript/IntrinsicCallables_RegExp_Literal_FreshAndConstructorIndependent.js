"use strict";

const first = /a+/g;
first.lastIndex = 2;
const second = /a+/g;

RegExp = function () {
    throw new Error("literal used the global constructor");
};

const third = /a+/g;
console.log(first !== second);
console.log(second.lastIndex);
console.log(third.test("aa"));
console.log(third.lastIndex);

// A literal that fails at runtime must throw a fresh SyntaxError on every evaluation.
function unsupported() {
    return /a/v;
}
let firstError;
try { unsupported(); } catch (e) { firstError = e; firstError.tag = "first"; }
try { unsupported(); } catch (e) {
    console.log(e instanceof SyntaxError);
    console.log(e !== firstError);
    console.log(e.tag);
}
