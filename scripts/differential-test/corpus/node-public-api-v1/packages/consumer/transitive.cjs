'use strict';
const h = require('../../harness.js');
h.finish(h.check([require('outer'), require('nested')], ['inner-dependency', 'consumer-dependency']));
