"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const testFile = path.join(os.tmpdir(), `test-stat-callback-${unique}.txt`);

fs.writeFileSync(testFile, 'hello', 'utf8');

fs.stat(testFile, (err, stats) => {
    if (err) {
        console.error('Error:', err.message);
    } else {
        console.log('Size:', stats.size);
    }
    fs.rmSync(testFile, { force: true });
});
