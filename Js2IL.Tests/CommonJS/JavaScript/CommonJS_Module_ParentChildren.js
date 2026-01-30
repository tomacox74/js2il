"use strict";\r\n\r\n// Main module that requires child modules to test parent/children relationships
// This is the entry point

const child1 = require('./CommonJS_Module_ParentChildren_Child1');
const child2 = require('./CommonJS_Module_ParentChildren_Child2');

// Main module should have null parent (it's the entry point)
console.log("main module.parent:", module.parent);

// Main module should have children array
console.log("main module.children type:", typeof module.children);
console.log("main module.children is array:", Array.isArray(module.children));
console.log("main module.children length:", module.children.length);

// The children should be the modules we required
if (module.children.length >= 2) {
    console.log("child 0 has id:", typeof module.children[0].id === "string");
    console.log("child 1 has id:", typeof module.children[1].id === "string");
}

console.log("child1 export:", child1.name);
console.log("child2 export:", child2.name);
