/*
 * Convert an extracted ECMA-262 multipage HTML fragment (typically produced by
 * scripts/extractEcma262SectionHtml.js --no-wrap) into a Markdown approximation.
 *
 * This is intentionally dependency-free; it handles the common HTML tags and
 * ecmarkup custom elements found in tc39.es.
 *
 * Usage:
 *   node scripts/convertEcmaExtractHtmlToMarkdown.js --in test_output/ecma262-27.3.html --out test_output/ecma262-27.3.md
 */

'use strict';

const fs = require('fs');
const path = require('path');

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

function decodeEntities(s) {
  return s
    .replace(/&nbsp;/g, ' ')
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&amp;/g, '&')
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'")
    .replace(/&#x([0-9a-fA-F]+);/g, (_, hex) => String.fromCharCode(parseInt(hex, 16)))
    .replace(/&#(\d+);/g, (_, dec) => String.fromCharCode(parseInt(dec, 10)));
}

function stripTags(s) {
  return s.replace(/<[^>]+>/g, '');
}

function convertHtmlToMarkdown(html) {
  let text = html.replace(/\r\n/g, '\n');

  // Convert <pre> blocks first, replacing them with placeholders.
  const preBlocks = [];
  text = text.replace(/<pre\b[\s\S]*?<\/pre>/gi, (block) => {
    const idx = preBlocks.length;

    const langMatch = /class=["'][^"']*language-([^"'\s]+)[^"']*["']/i.exec(block);
    const lang = (langMatch && langMatch[1]) || '';

    // Prefer inner <code> content if present.
    let inner = block;
    inner = inner.replace(/^[\s\S]*?<code\b[^>]*>/i, '');
    inner = inner.replace(/<\/code>[\s\S]*$/i, '');

    inner = decodeEntities(stripTags(inner));

    const fence = '```' + (lang ? ` ${lang}` : '');
    preBlocks.push(`\n${fence}\n${inner.trim()}\n\`\`\`\n`);

    return `@@PRE_${idx}@@`;
  });

  // Anchors
  text = text.replace(
    /<a\b[^>]*href=\"([^\"]+)\"[^>]*>([\s\S]*?)<\/a>/gi,
    (_, href, inner) => {
      const label = decodeEntities(stripTags(inner)).replace(/\s+/g, ' ').trim();
      return `[${label}](${href})`;
    }
  );

  // Headings (ecmarkup tends to use h1 repeatedly inside nested clauses)
  text = text.replace(/<h1\b[^>]*>/gi, '\n\n### ').replace(/<\/h1>/gi, '\n');
  text = text.replace(/<h2\b[^>]*>/gi, '\n\n#### ').replace(/<\/h2>/gi, '\n');
  text = text.replace(/<h3\b[^>]*>/gi, '\n\n##### ').replace(/<\/h3>/gi, '\n');

  // Paragraph-ish
  text = text.replace(/<p\b[^>]*>/gi, '\n\n').replace(/<\/p>/gi, '\n');
  text = text.replace(/<br\s*\/?>/gi, '\n');

  // Lists
  text = text.replace(/<li\b[^>]*>/gi, '\n- ').replace(/<\/li>/gi, '');
  text = text.replace(/<ul\b[^>]*>/gi, '\n').replace(/<\/ul>/gi, '\n');
  text = text.replace(/<ol\b[^>]*>/gi, '\n').replace(/<\/ol>/gi, '\n');

  // Inline code-ish elements
  text = text.replace(/<code\b[^>]*>/gi, '`').replace(/<\/code>/gi, '`');
  text = text.replace(/<var\b[^>]*>/gi, '`').replace(/<\/var>/gi, '`');
  text = text.replace(/<emu-val\b[^>]*>/gi, '`').replace(/<\/emu-val>/gi, '`');
  text = text.replace(/<emu-const\b[^>]*>/gi, '`').replace(/<\/emu-const>/gi, '`');

  // Drop remaining tags (including emu-* wrappers)
  text = stripTags(text);
  text = decodeEntities(text);

  // Restore pre blocks
  for (let i = 0; i < preBlocks.length; i++) {
    text = text.replace(`@@PRE_${i}@@`, preBlocks[i]);
  }

  // Cleanup
  text = text
    .replace(/[ \t]+\n/g, '\n')
    .replace(/\n{4,}/g, '\n\n\n')
    .trim();

  return text + '\n';
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
