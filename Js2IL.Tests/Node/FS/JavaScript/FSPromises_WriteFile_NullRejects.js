"use strict";
const fs = require('fs/promises');
const path = require('path');

const testFile = path.join(require('os').tmpdir(), 'test-writefile-null.txt');

fs.writeFile(testFile, null, 'utf8').then(() => {
    console.log('Unexpected success');
    require('fs').rmSync(testFile, { force: true });
}).catch(err => {
    console.log('Error name:', err.name);
    console.log('Error message:', err.message);
    require('fs').rmSync(testFile, { force: true });
});
