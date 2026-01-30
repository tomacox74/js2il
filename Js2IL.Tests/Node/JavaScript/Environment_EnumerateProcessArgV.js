"use strict";

console.log('argv length is', process.argv.length);
for (var i = 0; i < process.argv.length; i++) {
  console.log('arg', i, '=', process.argv[i]);
}
