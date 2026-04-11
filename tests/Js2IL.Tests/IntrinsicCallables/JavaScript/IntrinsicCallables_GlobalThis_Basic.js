"use strict";

// Ensure the globalThis identifier is accepted and returns a usable global object.
globalThis.console.log("globalThis ok");

// Property round-trip via the global object.
globalThis.__js2il_test = 123;
console.log(globalThis.__js2il_test);

// Self reference.
console.log(globalThis.globalThis === globalThis);

// Delegate-valued global functions on the global object.
console.log(globalThis.parseInt("11", 2));
