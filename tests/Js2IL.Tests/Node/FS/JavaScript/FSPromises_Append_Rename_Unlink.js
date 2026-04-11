"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

(async () => {
    const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
    const source = path.join(os.tmpdir(), `js2il-fs-append-${uniqueSuffix}.txt`);
    const renamed = path.join(os.tmpdir(), `js2il-fs-append-${uniqueSuffix}-renamed.txt`);

    try {
        fs.rmSync(source, { force: true });
        fs.rmSync(renamed, { force: true });

        await fs.promises.writeFile(source, 'a', 'utf8');
        await fs.promises.appendFile(source, 'b', 'utf8');
        await fs.promises.rename(source, renamed);

        const text = await fs.promises.readFile(renamed, 'utf8');
        console.log('Text:', text);

        await fs.promises.unlink(renamed);
        console.log('ExistsAfterUnlink:', fs.existsSync(renamed));
    } catch (err) {
        console.log('Error:', err && err.message);
    } finally {
        fs.rmSync(source, { force: true });
        fs.rmSync(renamed, { force: true });
    }
})();
