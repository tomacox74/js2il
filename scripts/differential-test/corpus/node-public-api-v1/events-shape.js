'use strict';
const events = require('node:events');
const h = require('./harness.js');
const emitter = new events.EventEmitter();
const listener = () => {};
const same = emitter.on('record', listener) === emitter;
h.finish(h.check([
  typeof events, events === events.EventEmitter, same,
  emitter.listeners('record')[0] === listener,
  emitter.listenerCount('record'), typeof emitter.once, typeof emitter.off
], ['function', true, true, true, 1, 'function', 'function']));
