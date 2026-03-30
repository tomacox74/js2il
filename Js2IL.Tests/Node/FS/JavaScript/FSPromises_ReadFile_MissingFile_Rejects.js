"use strict";
const fs = require('fs/promises');
const path = require('path');

const missing = path.join(require('os').tmpdir(), 'js2il-missing-file-does-not-exist.txt');

fs.readFile(missing, 'utf8')
    .then(() => {
        console.log('Unexpected success');
    })
    .catch(err => {
        console.log('Is ENOENT:', String(err.message).startsWith('ENOENT:'));
    });
