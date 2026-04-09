'use strict';

const fs = require('fs');
const path = require('path');

function normalize(text, outFile) {
  if (text.includes(' -> ')) {
    text = text.substring(0, text.indexOf(' -> ') + 4) + '<outFile>';
  }

  return text
    .replace(new RegExp(outFile.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), '<outFile>')
    .replace(/127\.0\.0\.1:\d+/g, '127.0.0.1:<port>')
    .replace(/\r\n/g, '\n');
}

async function run() {
  const indexUrl = process.argv[2];
  const outFile = path.join(process.cwd(), 'section-auto.html');
  const extractor = require('./Compile_Scripts_ExtractEcma262SectionHtml');
  const lines = [];
  const capture = (value) => {
    lines.push(String(value));
  };

  await extractor.main([
    'dotnet',
    'extractEcma262SectionHtml.js',
    '--section',
    '27.3',
    '--auto',
    '--index-url',
    indexUrl,
    '--out',
    outFile,
  ], capture);

  for (const line of lines) {
    console.log(normalize(line, outFile));
  }

  console.log(normalize(fs.readFileSync(outFile, 'utf8'), outFile));
}

run().catch((err) => {
  console.error(err && err.stack ? err.stack : (err && err.message ? err.message : err));
  process.exitCode = 1;
});
