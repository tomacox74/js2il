"use strict";\r\n\r\nconsole.log('argv length is', process.argv.length);
for (var i = 0; i < process.argv.length; i++) {
  console.log('arg', i, '=', process.argv[i]);
}
