'use strict';
const EventEmitter = require('node:events');
const h = require('./harness.js');
const emitter = new EventEmitter();
const original = new Error('intentional event error');
let sameError = false;
try { emitter.emit('error', original); } catch (failure) { sameError = failure === original; }
h.finish(h.check([
  sameError, h.error(() => emitter.on('record', null)),
  h.error(() => emitter.once('record', 4)),
  h.error(() => emitter.removeListener('record', false)),
  h.error(() => emitter.emit('error', 'unhandled payload'))
], [true, ['TypeError', 'ERR_INVALID_ARG_TYPE'], ['TypeError', 'ERR_INVALID_ARG_TYPE'],
  ['TypeError', 'ERR_INVALID_ARG_TYPE'], ['Error', 'ERR_UNHANDLED_ERROR']]));
