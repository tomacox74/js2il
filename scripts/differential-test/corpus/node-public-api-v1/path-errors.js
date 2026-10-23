'use strict';
const p = require('node:path');
const h = require('./harness.js');
h.finish(h.check([
  h.error(() => p.join('records', 17)),
  h.error(() => p.basename(null)),
  h.error(() => p.parse({})),
  h.error(() => p.relative('/records', false))
], Array(4).fill(['TypeError', 'ERR_INVALID_ARG_TYPE'])));
