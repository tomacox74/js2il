"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const testFile = path.join(os.tmpdir(), `test-readfile-callback-${unique}.txt`);

fs.writeFileSync(testFile, 'Hello, callback!', 'utf8');

fs.readFile(testFile, 'utf8', (err, content) => {
    if (err) {
        console.error('Error:', err.message);
    } else {
        console.log('Content:', content);
    }
    fs.rmSync(testFile, { force: true });
});
