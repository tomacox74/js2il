"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const src = path.join(os.tmpdir(), `test-copyfile-callback-src-${unique}.txt`);
const dst = path.join(os.tmpdir(), `test-copyfile-callback-dst-${unique}.txt`);

fs.writeFileSync(src, 'copied content', 'utf8');

fs.copyFile(src, dst, (err) => {
    if (err) {
        console.error('Error:', err.message);
    } else {
        const content = fs.readFileSync(dst, 'utf8');
        console.log('Copied content:', content);
    }

    fs.rmSync(src, { force: true });
    fs.rmSync(dst, { force: true });
});
