"use strict";
const fs = require('fs/promises');
const syncFs = require('fs');
const path = require('path');
const os = require('os');

const dir = path.join(os.tmpdir(), 'js2il-readdir-types-' + Date.now());
syncFs.mkdirSync(dir, { recursive: true });
syncFs.writeFileSync(path.join(dir, 'alpha.txt'), '', 'utf8');
syncFs.mkdirSync(path.join(dir, 'subdir'));

fs.readdir(dir, { withFileTypes: true })
    .then(entries => {
        const lines = entries.map(e => e.name + ':' + (e.isDirectory() ? 'dir' : 'file'));
        lines.sort();
        lines.forEach(l => console.log(l));
    })
    .catch(err => {
        console.error('Error:', err.message);
    })
    .finally(() => {
        syncFs.rmSync(dir, { recursive: true, force: true });
    });
