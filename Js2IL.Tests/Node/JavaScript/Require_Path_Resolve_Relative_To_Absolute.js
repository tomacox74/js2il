"use strict";\r\n\r\nconst path = require('path');
const resolved = path.resolve(__dirname, 'a', '..', 'b', 'c');
// Stable output: only final segment
console.log(path.basename(resolved));
