/*
 * Convert an extracted ECMA-262 multipage HTML fragment (typically produced by
 * scripts/extractEcma262SectionHtml.js --no-wrap) into a Markdown approximation.
 *
 * This uses `turndown` to convert HTML to Markdown, with a few custom rules to
 * better handle ecmarkup elements and code blocks.
 *
 * Usage:
 *   node scripts/convertEcmaExtractHtmlToMarkdown.js --in test_output/ecma262-27.3.html --out test_output/ecma262-27.3.md
 */

'use strict';

const fs = require('fs');
const path = require('path');
const TurndownService = require('turndown');

function parseArgs(argv) {
  const args = { inFile: '', outFile: '' };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];

    if (a === '--in' || a === '-i') {
      args.inFile = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--in=')) {
      args.inFile = a.substring('--in='.length);
      continue;
    }

    if (a === '--out' || a === '-o') {
      args.outFile = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--out=')) {
      args.outFile = a.substring('--out='.length);
      continue;
    }
  }

  return args;
}

function convertHtmlToMarkdown(html) {
  const turndownService = new TurndownService({
    codeBlockStyle: 'fenced',
    emDelimiter: '_',
    strongDelimiter: '**',
    bulletListMarker: '-',
  });

  // tc39/ecmarkup uses nested h1s heavily; prefer lower heading levels to keep output readable.
  turndownService.addRule('ecmaHeadings', {
    filter: ['h1', 'h2', 'h3'],
    replacement(content, node) {
      const tag = (node.nodeName || '').toLowerCase();
      const originalLevel = tag === 'h1' ? 1 : tag === 'h2' ? 2 : 3;
      const mappedLevel = Math.min(6, 2 + originalLevel); // h1->###, h2->####, h3->#####
      const hashes = '#'.repeat(mappedLevel);
      const clean = (content || '').replace(/\s+/g, ' ').trim();
      return `\n\n${hashes} ${clean}\n\n`;
    },
  });

  // Wrap common ecmarkup inline elements as code.
  turndownService.addRule('ecmarkupInlineCode', {
    filter: ['emu-val', 'emu-const', 'var'],
    replacement(content) {
      const clean = (content || '').replace(/\s+/g, ' ').trim();
      return clean ? `\`${clean}\`` : '';
    },
  });

  // Prefer fenced code blocks and preserve language hints (class="language-xxx") when present.
  turndownService.addRule('fencedCodeWithLanguage', {
    filter(node) {
      return node.nodeName === 'PRE';
    },
    replacement(_content, node) {
      // Use textContent to avoid turndown escaping backticks inside code.
      const raw = (node.textContent || '').replace(/\r\n/g, '\n');

      // Try to find language-xxx on either <pre> or a nested <code>.
      const langFromClass = (el) => {
        const className = el && el.getAttribute ? el.getAttribute('class') : '';
        const m = className && /(?:^|\s)language-([^\s]+)/i.exec(className);
        return (m && m[1]) || '';
      };

      let lang = langFromClass(node);
      if (!lang && node.querySelector) {
        const code = node.querySelector('code');
        if (code) lang = langFromClass(code);
      }

      const fence = `\n\n\`\`\`${lang ? ` ${lang}` : ''}\n`;
      const body = raw.replace(/\n$/, '');
      return `${fence}${body}\n\`\`\`\n\n`;
    },
  });

  const md = turndownService.turndown(html);
  return md.replace(/\n{4,}/g, '\n\n\n').trim() + '\n';
}

function main() {
  const args = parseArgs(process.argv);

  if (!args.inFile) throw new Error('Missing --in <input.html>');
  if (!args.outFile) throw new Error('Missing --out <output.md>');

  const inPath = path.resolve(process.cwd(), args.inFile);
  const outPath = path.resolve(process.cwd(), args.outFile);

  const html = fs.readFileSync(inPath, 'utf8');
  const md = convertHtmlToMarkdown(html);

  fs.mkdirSync(path.dirname(outPath), { recursive: true });
  fs.writeFileSync(outPath, md, 'utf8');

  // eslint-disable-next-line no-console
  console.log(`Converted ${args.inFile} -> ${args.outFile} (${md.split(/\n/).length} lines)`);
}

if (require.main === module) {
  try {
    main();
  } catch (err) {
    // eslint-disable-next-line no-console
    console.error(err && err.message ? err.message : err);
    process.exitCode = 1;
  }
}
