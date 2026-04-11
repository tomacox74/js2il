"use strict";
const fs = require('fs/promises');
const path = require('path');
const os = require('os');

const missing = path.join(os.tmpdir(), 'js2il-readdir-missing-does-not-exist-' + Date.now());

fs.readdir(missing)
    .then(() => {
        console.log('Unexpected success');
    })
    .catch(err => {
        console.log('Is ENOENT:', String(err.message).startsWith('ENOENT:'));
    });
