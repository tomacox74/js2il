'use strict';
const EventEmitter = require('node:events');
const h = require('./harness.js');
const emitter = new EventEmitter();
let once = 0;
let regular = 0;
function listener() { regular++; }
emitter.once('record', () => { once++; emitter.emit('record'); });
emitter.on('record', listener);
emitter.emit('record');
emitter.removeListener('record', listener);
emitter.emit('record');
h.finish(h.check([once, regular, emitter.listenerCount('record'), emitter.eventNames()], [1, 2, 0, []]));
