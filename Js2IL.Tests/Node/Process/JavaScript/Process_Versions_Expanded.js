"use strict";

const hasNodeVersion = typeof process.versions.node === "string" && process.versions.node.length > 0;
const hasV8Version = typeof process.versions.v8 === "string" && process.versions.v8.length > 0;
const hasModulesVersion = typeof process.versions.modules === "string" && process.versions.modules.length > 0;
const hasJs2ilVersion = typeof process.versions.js2il === "string" && process.versions.js2il.length > 0;
const hasDotnetVersion = typeof process.versions.dotnet === "string" && process.versions.dotnet.length > 0;

console.log("has versions.node", hasNodeVersion);
console.log("has versions.v8", hasV8Version);
console.log("has versions.modules", hasModulesVersion);
console.log("has versions.js2il", hasJs2ilVersion);
console.log("has versions.dotnet", hasDotnetVersion);

// Verify that node version is the target version
const nodeVersion = process.versions.node;
console.log("node version format valid", /^\d+\.\d+\.\d+$/.test(nodeVersion));
