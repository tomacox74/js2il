'use strict';
const Buffer = require('node:buffer').Buffer;
const h = require('./harness.js');
const bytes = Buffer.alloc(8);
const end = bytes.writeUInt32LE(4275878552, 0);
bytes.writeUInt16BE(4660, 4);
const view = bytes.subarray(4, 6);
view[0] = 86;
h.finish(h.check([
  end, bytes.readUInt32LE(0), bytes.readUInt16BE(4),
  bytes.toString('hex'), view.toString('hex'),
  Buffer.alloc(0).length, Buffer.from([]).length
], [4, 4275878552, 22068, '98badcfe56340000', '5634', 0, 0]));
