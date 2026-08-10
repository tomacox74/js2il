"use strict";

function passthrough(value) {
    return value;
}

function readCapturedRequire() {
    return require;
}

const first = require;
const passed = passthrough(require);
const holder = { value: require };

console.log(typeof require);
console.log(first === require);
console.log(passed === require);
console.log(holder.value === require);
console.log(readCapturedRequire() === require);
console.log(module.require === require);
console.log(require.main === module);
console.log(module.require.main === module);
console.log(require.name);
console.log(require.length);

const direct = require("./CommonJS_Require_FunctionValueIdentity_Dependency");
const indirect = passed("./CommonJS_Require_FunctionValueIdentity_Dependency");

console.log(direct === indirect);
console.log(indirect.answer);
console.log(typeof indirect.requireValue);
console.log(indirect.requireValue === indirect.readRequire());
console.log(indirect.requireValue === require);
