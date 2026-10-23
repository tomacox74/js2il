// Original runner-validation fixture, not an upstream conformance corpus.
const fs = require('fs');
const buffer = require('buffer');
const error = new TypeError('controlled');
error.code = 'E_CONTROLLED';
fs.writeFileSync('values.json', JSON.stringify({
  number: 42,
  string: '42',
  boolean: true,
  nullValue: null,
  undefinedValue: { type: typeof undefined },
  negativeZero: { type: 'number', value: 1 / -0 === -Infinity ? '-0' : 'other' },
  error: { name: error.name, code: error.code, message: error.message }
}));
fs.writeFileSync('effect.bin', buffer.Buffer.from([0, 255, 13, 10]));
fs.writeFileSync('events.json', JSON.stringify(['begin', 'ready', 'done']));
console.log('runner smoke');
console.error('controlled stderr');
