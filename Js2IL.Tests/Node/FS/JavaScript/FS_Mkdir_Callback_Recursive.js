"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const base = path.join(os.tmpdir(), 'js2il-mkdir-callback');
const target = path.join(base, 'nested');

fs.rmSync(base, { recursive: true, force: true });

fs.mkdir(target, { recursive: true }, (err) => {
    console.log('ErrIsNull:', err == null);
    console.log('Exists:', fs.existsSync(target));
    fs.rmSync(base, { recursive: true, force: true });
});
