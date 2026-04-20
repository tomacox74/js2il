"use strict";

const test = Function("var desc = Object.getOwnPropertyDescriptor(arguments, 'callee');console.log(desc.configurable);console.log(desc.enumerable);console.log(desc.writable);console.log(typeof Object.getOwnPropertyDescriptor(desc, 'get') === 'undefined');console.log(typeof Object.getOwnPropertyDescriptor(desc, 'set') === 'undefined');console.log(desc.value === arguments.callee);");

test();
