"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const testFile = path.join(os.tmpdir(), `test-writefile-callback-${unique}.txt`);

fs.writeFile(testFile, 'Written by fs.writeFile', 'utf8', (err) => {
    if (err) {
        console.error('Error:', err.message);
    } else {
        const content = fs.readFileSync(testFile, 'utf8');
        console.log('Content:', content);
    }
    fs.rmSync(testFile, { force: true });
});
