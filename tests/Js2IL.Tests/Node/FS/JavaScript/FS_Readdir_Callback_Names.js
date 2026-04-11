"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const dir = path.join(os.tmpdir(), `test-readdir-callback-${unique}`);

fs.mkdirSync(dir, { recursive: true });

const f1 = path.join(dir, 'a.txt');
const f2 = path.join(dir, 'b.txt');
fs.writeFileSync(f1, 'a', 'utf8');
fs.writeFileSync(f2, 'b', 'utf8');

fs.readdir(dir, (err, entries) => {
    if (err) {
        console.error('Error:', err.message);
    } else {
        entries.sort();
        console.log('Entries:', entries.join(','));
    }

    fs.rmSync(dir, { force: true });
});
