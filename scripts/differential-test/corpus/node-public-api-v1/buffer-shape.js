'use strict';
const exported = require('node:buffer');
const h = require('./harness.js');
const value = exported.Buffer.from([7, 8]);
h.finish(h.check([
  exported.Buffer === Buffer, typeof exported.Buffer.from, typeof exported.Buffer.alloc,
  typeof value.subarray, typeof value.readUInt16LE,
  exported.Buffer.isBuffer(value), exported.Buffer.isBuffer(new Uint8Array(2)),
  value instanceof Uint8Array, value.toJSON()
], [true, 'function', 'function', 'function', 'function', true, false, true,
  { type: 'Buffer', data: [7, 8] }]));
