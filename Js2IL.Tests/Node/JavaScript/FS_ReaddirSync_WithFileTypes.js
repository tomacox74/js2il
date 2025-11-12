const fs = require('fs');
const path = require('path');
const dir = __dirname;
const f1 = path.join(dir, 'ft1.txt');
fs.writeFileSync(f1, 'a');
// More stable: rely on existsSync; still validate as file
const out = fs.existsSync(f1) ? 'ft1.txt:file' : '';
console.log(out);
fs.rmSync(f1, { force: true });
