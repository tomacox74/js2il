"use strict";
const fs = require('fs/promises');
const path = require('path');

const testFile = path.join(require('os').tmpdir(), 'test-writefile.txt');

fs.writeFile(testFile, 'Written by fs/promises', 'utf8').then(() => {
    const content = require('fs').readFileSync(testFile, 'utf8');
    console.log('Written content:', content);
    require('fs').rmSync(testFile);
}).catch(err => {
    console.error('Error:', err.message);
});
