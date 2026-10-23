'use strict';
const EventEmitter = require('node:events');
const fs = require('node:fs');
const h = require('./harness.js');
const emitter = new EventEmitter();
const order = ['before'];
let receiver = false;
emitter.on('record', function (value) { receiver = this === emitter; order.push('normal-' + value); });
emitter.prependListener('record', () => order.push('prepended'));
const emitted = emitter.emit('record', 9);
order.push('after');
fs.writeFileSync('order.json', JSON.stringify(order));
h.finish(h.check([emitted, receiver, emitter.emit('absent'), order],
  [true, true, false, ['before', 'prepended', 'normal-9', 'after']]));
