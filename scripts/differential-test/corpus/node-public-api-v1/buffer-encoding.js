'use strict';
const Buffer = require('node:buffer').Buffer;
const fs = require('node:fs');
const h = require('./harness.js');
const bytes = Buffer.from('A\u00e9', 'utf8');
fs.writeFileSync('effect.bin', bytes);
h.finish(h.check([
  bytes.length, bytes.toString('hex'), bytes.toString('base64'),
  Buffer.from('4100ff', 'hex').toString('hex'),
  Buffer.byteLength('A\u00e9', 'utf8'),
  Buffer.from([0, 255, 256, -1]).toString('hex')
], [3, '41c3a9', 'QcOp', '4100ff', 3, '00ff00ff']));
