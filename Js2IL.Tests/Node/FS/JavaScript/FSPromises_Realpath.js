"use strict";
const fs = require('fs/promises');
const path = require('path');

const testFile = path.join(require('os').tmpdir(), 'test-realpath.txt');
require('fs').writeFileSync(testFile, 'test', 'utf8');

fs.realpath(testFile).then(fullPath => {
    console.log('Real path exists:', fullPath.length > 0);
    console.log('Contains filename:', fullPath.includes('test-realpath.txt'));
    require('fs').rmSync(testFile);
}).catch(err => {
    console.error('Error:', err.message);
});
