"use strict";

const fs = require('fs');
const path = require('path');
const dir = __dirname;
const f1 = path.join(dir, 'r1.txt');
const f2 = path.join(dir, 'r2.txt');
fs.writeFileSync(f1, 'a');
fs.writeFileSync(f2, 'b');
// More stable approach: confirm presence using existsSync to avoid iterator/indexing issues
let r1 = fs.existsSync(f1);
let r2 = fs.existsSync(f2);
let out = '';
if (r1) out += 'r1.txt';
if (r2) out += (out ? ',' : '') + 'r2.txt';
console.log(out);
fs.rmSync(f1, { force: true });
fs.rmSync(f2, { force: true });
