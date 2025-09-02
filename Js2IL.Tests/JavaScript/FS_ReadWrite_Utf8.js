const fs = require('fs');
const path = require('path');

const tmp = path.join(__dirname, 'utf8-test.txt');
fs.writeFileSync(tmp, 'héllo–世界', 'utf8');
const text = fs.readFileSync(tmp, 'utf8');
console.log(text);
