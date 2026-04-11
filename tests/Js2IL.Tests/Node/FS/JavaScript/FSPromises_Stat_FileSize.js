"use strict";
const fs = require('fs/promises');
const path = require('path');

const testFile = path.join(require('os').tmpdir(), 'test-stat.txt');
const testContent = 'Test content for stat';
require('fs').writeFileSync(testFile, testContent, 'utf8');

fs.stat(testFile).then(stats => {
    console.log('File size:', stats.size);
    require('fs').rmSync(testFile);
}).catch(err => {
    console.error('Error:', err.message);
});
