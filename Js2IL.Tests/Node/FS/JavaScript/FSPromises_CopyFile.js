"use strict";
const fs = require('fs/promises');
const syncFs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const src = path.join(os.tmpdir(), `test-copyfile-src-${unique}.txt`);
const dst = path.join(os.tmpdir(), `test-copyfile-dst-${unique}.txt`);

syncFs.writeFileSync(src, 'copied content', 'utf8');

fs.copyFile(src, dst)
    .then(() => {
        const content = syncFs.readFileSync(dst, 'utf8');
        console.log('Copied content:', content);
    })
    .catch(err => {
        console.error('Error:', err.message);
    })
    .finally(() => {
        syncFs.rmSync(src, { force: true });
        syncFs.rmSync(dst, { force: true });
    });
