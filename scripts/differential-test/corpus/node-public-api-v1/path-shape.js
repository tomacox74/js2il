'use strict';
const p = require('node:path');
const h = require('./harness.js');
const parsed = p.posix.parse('/records/archive.tar.gz');
h.finish(h.check([
  p.sep, p.delimiter, typeof p.join, typeof p.posix.join, typeof p.win32.join,
  parsed.root, parsed.dir, parsed.base, parsed.ext, parsed.name,
  p.posix.format(parsed), p.posix.basename('/records/archive.tar.gz', '.gz'),
  p.posix.extname('.hidden'), p.posix.extname('record.')
], ['/', ':', 'function', 'function', 'function', '/', '/records', 'archive.tar.gz',
  '.gz', 'archive.tar', '/records/archive.tar.gz', 'archive.tar', '', '.']));
