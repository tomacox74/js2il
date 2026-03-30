"use strict";
const fs = require('fs/promises');
const syncFs = require('fs');
const path = require('path');
const os = require('os');

const dir = path.join(os.tmpdir(), 'js2il-readdir-names-' + Date.now());
syncFs.mkdirSync(dir, { recursive: true });
syncFs.writeFileSync(path.join(dir, 'alpha.txt'), '', 'utf8');
syncFs.writeFileSync(path.join(dir, 'beta.txt'), '', 'utf8');
syncFs.mkdirSync(path.join(dir, 'subdir'));

fs.readdir(dir)
    .then(entries => {
        const sorted = entries.slice().sort();
        sorted.forEach(e => console.log(e));
    })
    .catch(err => {
        console.error('Error:', err.message);
    })
    .finally(() => {
        syncFs.rmSync(dir, { recursive: true, force: true });
    });
