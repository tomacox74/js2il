'use strict';
const h = require('./harness.js');
const first = require('./modules/cached.js');
first.value = 23;
const second = require('./modules/cached.js');
h.finish(h.check([first === second, second.value, globalThis.__nodeCorpusInitializations], [true, 23, 1]));
