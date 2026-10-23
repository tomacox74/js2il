'use strict';
const Buffer = require('node:buffer').Buffer;
const h = require('./harness.js');
h.finish(h.check([
  h.error(() => Buffer.from(null)),
  h.error(() => Buffer.from('record', 'invalid-encoding')),
  h.error(() => Buffer.alloc(-1)),
  h.error(() => Buffer.alloc(1).readUInt16LE(0))
], [['TypeError', 'ERR_INVALID_ARG_TYPE'], ['TypeError', 'ERR_UNKNOWN_ENCODING'],
  ['RangeError', 'ERR_OUT_OF_RANGE'], ['RangeError', 'ERR_BUFFER_OUT_OF_BOUNDS']]));
