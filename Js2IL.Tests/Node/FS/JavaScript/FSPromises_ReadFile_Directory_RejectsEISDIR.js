"use strict";
const fs = require('fs/promises');
const path = require('path');
const os = require('os');
const syncFs = require('fs');

const dirPath = path.join(os.tmpdir(), 'js2il-readfile-dir-test');
syncFs.mkdirSync(dirPath, { recursive: true });

fs.readFile(dirPath, 'utf8')
    .then(() => {
        console.log('Unexpected success');
    })
    .catch(err => {
        console.log('Is EISDIR:', String(err.message).startsWith('EISDIR:'));
    })
    .finally(() => {
        syncFs.rmSync(dirPath, { recursive: true, force: true });
    });
