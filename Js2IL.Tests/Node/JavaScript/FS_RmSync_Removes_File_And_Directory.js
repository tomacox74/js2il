const fs = require('fs');
const path = require('path');
const file = path.join(__dirname, 'deleteme.txt');
fs.writeFileSync(file, 'x');
console.log('before', fs.existsSync(file));
fs.rmSync(file, {});
console.log('after', fs.existsSync(file));
// Force removal of a non-existent path should not throw
fs.rmSync(path.join(__dirname, 'no_such_dir'), { force: true });
console.log('force');
