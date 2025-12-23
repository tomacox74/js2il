// Child module 1 - tests module.parent

// This module's parent should be the main module
console.log("child1 module.parent is not null:", module.parent !== null);
console.log("child1 module.parent type:", typeof module.parent);

// Parent should have id property
if (module.parent) {
    console.log("child1 parent has id:", typeof module.parent.id === "string");
}

// Export something
exports.name = "child1";
