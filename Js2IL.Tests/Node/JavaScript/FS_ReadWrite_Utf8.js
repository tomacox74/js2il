"use strict";

const fs = require('fs');
const path = require('path');

const file = path.join(__dirname, 'tmp.txt');
fs.writeFileSync(file, 'hello', 'utf8');
const content = fs.readFileSync(file, 'utf8');
console.log(content);
