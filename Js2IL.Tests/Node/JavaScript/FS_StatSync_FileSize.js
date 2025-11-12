const fs = require('fs');
const path = require('path');
const file = path.join(__dirname, 'size.txt');
fs.writeFileSync(file, 'hello!');
const st = fs.statSync(file);
console.log(st.size);
fs.rmSync(file, { force: true });
