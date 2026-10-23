'use strict';
const p = require('node:path').win32;
const h = require('./harness.js');
h.finish(h.check([
  p.join('D:\\sample', '..', 'final', 'item.log'),
  p.normalize('D:\\sample\\\\draft\\..\\item.log'),
  p.relative('D:\\alpha\\beta', 'D:\\alpha\\gamma'),
  p.isAbsolute('D:\\records'), p.isAbsolute('D:records')
], ['D:\\final\\item.log', 'D:\\sample\\item.log', '..\\gamma', true, false]));
