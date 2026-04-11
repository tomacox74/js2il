"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

(async () => {
    const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
    const file = path.join(os.tmpdir(), `js2il-fs-open-${uniqueSuffix}.txt`);

    try {
        fs.rmSync(file, { force: true });

        const handle = await fs.promises.open(file, 'w+');
        const writeResult = await handle.write('hello', 0, 'utf8');

        const buffer = Buffer.alloc(5);
        const readResult = await handle.read(buffer, 0, buffer.length, 0);
        await handle.close();

        console.log('FDIsNumber:', typeof handle.fd === 'number');
        console.log('BytesWritten:', writeResult.bytesWritten);
        console.log('BytesRead:', readResult.bytesRead);
        console.log('BufferText:', buffer.toString('utf8'));
    } catch (err) {
        console.log('Error:', err && err.message);
    } finally {
        fs.rmSync(file, { force: true });
    }
})();
