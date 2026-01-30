"use strict";

const fs = require('fs');
const path = require('path');
const dir = __dirname;
const file = path.join(dir, 'temp_exists.txt');
fs.writeFileSync(file, 'data');
console.log(fs.existsSync(dir));
console.log(fs.existsSync(file));
fs.rmSync(file, { force: true });
console.log(fs.existsSync(file));
