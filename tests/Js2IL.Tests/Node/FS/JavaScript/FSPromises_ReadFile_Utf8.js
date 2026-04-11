"use strict";
const fs = require('fs/promises');
const path = require('path');

const testFile = path.join(require('os').tmpdir(), 'test-readfile.txt');
require('fs').writeFileSync(testFile, 'Hello, fs/promises!', 'utf8');

fs.readFile(testFile, 'utf8').then(content => {
    console.log('Content:', content);
    require('fs').rmSync(testFile);
}).catch(err => {
    console.error('Error:', err.message);
});
