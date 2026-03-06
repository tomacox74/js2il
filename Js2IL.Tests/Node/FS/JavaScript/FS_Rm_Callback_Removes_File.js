"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const unique = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-rm-callback-${unique}.txt`);

fs.writeFileSync(file, 'x', 'utf8');

fs.rm(file, (err) => {
    console.log('ErrIsNull:', err == null);
    console.log('Exists:', fs.existsSync(file));
});
