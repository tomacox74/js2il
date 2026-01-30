"use strict";\r\n\r\nfunction bump(kind) {
  let maj = '3';
  let min = '2';
  let pat = '1';

  let M = maj;
  let m = min;
  let p = pat;

  switch (kind) {
    case 'major':
      --M;
      m = '0';
      p = '0';
      break;
    case 'minor':
      --m;
      p = '0';
      break;
    case 'patch':
      --p;
      break;
  }

  console.log(kind, M, m, p);
}

bump('major');
bump('minor');
bump('patch');
