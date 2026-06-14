"use strict";

const hasNodeVersion = typeof process.versions.node === "string" && process.versions.node.length > 0;
const hasV8Version = typeof process.versions.v8 === "string" && process.versions.v8.length > 0;
const hasModulesVersion = typeof process.versions.modules === "string" && process.versions.modules.length > 0;
const hasJrocVersion = typeof process.versions.jroc === "string" && process.versions.jroc.length > 0;
const hasDotnetVersion = typeof process.versions.dotnet === "string" && process.versions.dotnet.length > 0;

console.log("has versions.node", hasNodeVersion);
console.log("has versions.v8", hasV8Version);
console.log("has versions.modules", hasModulesVersion);
console.log("has versions.jroc", hasJrocVersion);
console.log("has versions.dotnet", hasDotnetVersion);

// Verify that node version is the target version
const nodeVersion = process.versions.node;
console.log("node version format valid", /^\d+\.\d+\.\d+$/.test(nodeVersion));
