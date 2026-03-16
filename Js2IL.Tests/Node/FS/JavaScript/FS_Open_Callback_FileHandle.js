"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-open-callback-${uniqueSuffix}.txt`);
fs.rmSync(file, { force: true });

fs.open(file, 'w+', (err, handle) => {
    if (err) {
        console.log('OpenError:', err.message);
        fs.rmSync(file, { force: true });
        return;
    }

    console.log('FDIsNumber:', typeof handle.fd === 'number');

    handle.write('ok', 0, 'utf8').then(() => handle.close()).then(() => {
        console.log('FileText:', fs.readFileSync(file, 'utf8'));
        fs.rmSync(file, { force: true });
    }, (writeErr) => {
        console.log('WriteError:', writeErr && writeErr.message);
        fs.rmSync(file, { force: true });
    });
});
