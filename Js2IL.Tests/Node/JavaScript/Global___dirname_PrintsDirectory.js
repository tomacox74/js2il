"use strict";

// The test harness runs scripts from a per-run temp directory ending in a GUID.
// Print the parent directory for stable output.
const path = require('path');
console.log(path.dirname(__dirname));
