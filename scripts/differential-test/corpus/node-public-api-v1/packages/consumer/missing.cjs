'use strict';
const h = require('../../harness.js');
h.finish(h.check([
  h.error(() => require('conditional/private.cjs')),
  h.error(() => require('./does-not-exist.cjs'))
], [['Error', 'ERR_PACKAGE_PATH_NOT_EXPORTED'], ['Error', 'MODULE_NOT_FOUND']]));
