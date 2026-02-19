"use strict";
const fs = require('fs/promises');
const path = require('path');

const testFile = path.join(require('os').tmpdir(), 'test-readfile-buffer.txt');
require('fs').writeFileSync(testFile, 'Buffer test', 'utf8');

fs.readFile(testFile).then(buffer => {
    console.log('Is Buffer:', Buffer.isBuffer(buffer));
    console.log('Length:', buffer.length);
    require('fs').rmSync(testFile);
}).catch(err => {
    console.error('Error:', err.message);
});
