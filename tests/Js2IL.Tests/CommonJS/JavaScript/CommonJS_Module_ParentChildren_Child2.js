"use strict";

// Child module 2 - tests module.parent

// This module's parent should be the main module
console.log("child2 module.parent is not null:", module.parent !== null);
console.log("child2 module.parent type:", typeof module.parent);

// Parent should have id property
if (module.parent) {
    console.log("child2 parent has id:", typeof module.parent.id === "string");
}

// Export something
exports.name = "child2";
