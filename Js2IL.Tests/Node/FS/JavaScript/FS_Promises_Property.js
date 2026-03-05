"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

console.log('HasPromises:', !!fs.promises);

const file = path.join(os.tmpdir(), 'js2il-fs-promises-smoke.txt');
fs.rmSync(file, { force: true });
fs.writeFileSync(file, 'x', 'utf8');

fs.promises.readFile(file, 'utf8').then((txt) => {
    console.log('ReadOk:', txt === 'x');
    fs.rmSync(file, { force: true });
}, (err) => {
    console.log('ReadOk:', false);
    console.log('Error:', err && err.message);
    fs.rmSync(file, { force: true });
});
