"use strict";

const isKnownPlatform = process.platform === "win32"
  || process.platform === "linux"
  || process.platform === "darwin"
  || process.platform === "unknown";

const hasNodeVersion = typeof process.versions === "object"
  && process.versions !== null
  && typeof process.versions.node === "string"
  && process.versions.node.length > 0;

const hasEnvObject = typeof process.env === "object"
  && process.env !== null
  && Object.keys(process.env).length > 0;

console.log("known platform", isKnownPlatform);
console.log("has versions.node", hasNodeVersion);
console.log("has env", hasEnvObject);
