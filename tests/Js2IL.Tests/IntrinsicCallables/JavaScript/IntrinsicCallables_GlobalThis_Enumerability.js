"use strict";

// Per ECMA-262, built-in globals (including globalThis itself) are non-enumerable.
// User-defined properties created by assignment are enumerable.

let sawGlobalThis = false;
for (const k in globalThis) {
  if (k === "globalThis") {
    sawGlobalThis = true;
  }
}
console.log(sawGlobalThis);

// User property should be enumerable.
globalThis.__js2il_enum_test = 1;

let sawUserProp = false;
for (const k in globalThis) {
  if (k === "__js2il_enum_test") {
    sawUserProp = true;
  }
}
console.log(sawUserProp);
