"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `test-realpath-callback-${unique}.txt`);

fs.writeFileSync(file, 'x', 'utf8');

fs.realpath(file, (err, resolved) => {
    console.log('ErrIsNull:', err == null);
    const norm = resolved ? resolved.split('\\').join('/') : '';
    console.log('EndsWith:', norm.endsWith(`/test-realpath-callback-${unique}.txt`));
    fs.rmSync(file, { force: true });
});
