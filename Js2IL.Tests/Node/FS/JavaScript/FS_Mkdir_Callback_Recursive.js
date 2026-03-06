"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const base = path.join(os.tmpdir(), `js2il-mkdir-callback-${unique}`);
const target = path.join(base, 'nested');

fs.rmSync(base, { recursive: true, force: true });

fs.mkdir(target, { recursive: true }, (err) => {
    console.log('ErrIsNull:', err == null);
    console.log('Exists:', fs.existsSync(target));
    fs.rmSync(base, { recursive: true, force: true });
});
