"use strict";\r\n\r\n// Test module identity properties: id, filename, path

// module.id should be the resolved filename (or '.' for main module)
console.log("module.id type:", typeof module.id);
console.log("module.id is string:", typeof module.id === "string");

// module.filename should be the fully-resolved filename of the module
console.log("module.filename type:", typeof module.filename);
console.log("module.filename is string:", typeof module.filename === "string");

// module.path should be the directory name of the module
console.log("module.path type:", typeof module.path);
console.log("module.path is string:", typeof module.path === "string");

// The path should be a prefix of the filename (directory of the file)
// We can't check exact values since they vary by machine, but we can verify types
console.log("all identity properties set:", 
    module.id !== undefined && 
    module.filename !== undefined && 
    module.path !== undefined);
