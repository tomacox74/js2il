'use strict';
const h = require('../../harness.js');
h.finish(h.check([require('conditional'), require('conditional/feature')], ['require-selected', 'public-feature']));
