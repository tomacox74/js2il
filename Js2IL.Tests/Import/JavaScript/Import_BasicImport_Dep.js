"use strict";
// Dependency module for Import_BasicImport test

exports.message = "Hello from imported module";
exports.getValue = function() {
    return 42;
};

console.log("Import_BasicImport_Dep loaded");
