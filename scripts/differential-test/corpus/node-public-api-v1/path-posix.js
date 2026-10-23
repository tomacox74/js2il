'use strict';
const p = require('node:path').posix;
const h = require('./harness.js');
h.finish(h.check([
  p.join('/records', 'draft', '..', 'final.txt'),
  p.normalize('/records//draft/../final/'),
  p.relative('/records/draft', '/records/final/item'),
  p.join(''), p.normalize(''), p.isAbsolute('/records'), p.isAbsolute('records')
], ['/records/final.txt', '/records/final/', '../final/item', '.', '.', true, false]));
