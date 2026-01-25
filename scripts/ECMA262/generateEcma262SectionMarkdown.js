/*
 * Generates docs/ECMA262/<section>/Section<section>_<sub>.md from a sibling JSON file.
 *
 * Usage:
 *   node scripts/ECMA262/generateEcma262SectionMarkdown.js --section 15.7
 *
 * Input:
 *   docs/ECMA262/15/Section15_7.json
 * Output:
 *   docs/ECMA262/15/Section15_7.md
 */

'use strict';

const fs = require('fs');
const path = require('path');

const DEFAULT_ROOT = path.resolve(__dirname, '..', '..', 'docs', 'ECMA262');
const AUTO_GENERATED_MARKER = '<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->';

function parseArgs(argv) {
  const args = {
    section: '',
    root: '',
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];

    if (a === '--help' || a === '-h') {
      args.help = true;
      continue;
    }

    if (a === '--section' || a === '-s') {
      args.section = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--section=')) {
      args.section = a.substring('--section='.length);
      continue;
    }

    if (a === '--root' || a === '-r') {
      args.root = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--root=')) {
      args.root = a.substring('--root='.length);
      continue;
    }

    // Allow positional section argument: `node ... 15.7`
    if (!a.startsWith('-') && !args.section) {
      args.section = a;
      continue;
    }
  }

  return args;
}

function detectEol(text) {
  return text.includes('\r\n') ? '\r\n' : '\n';
}

function readText(filePath) {
  return fs.readFileSync(filePath, 'utf8');
}

function writeTextPreserveEol(filePath, lines) {
  let eol = '\n';
  let existingText = null;

  if (fs.existsSync(filePath)) {
    existingText = fs.readFileSync(filePath, 'utf8');
    eol = detectEol(existingText);
  }

  const newText = lines.join(eol) + eol;

  if (existingText !== null && existingText === newText) {
    return false;
  }

  fs.writeFileSync(filePath, newText, 'utf8');
  return true;
}

function usage() {
  console.log('Generate a subsection markdown file from its JSON description.');
  console.log('');
   console.log('  node scripts/ECMA262/generateEcma262SectionMarkdown.js --section 15.7');
  console.log('');
  console.log('Options:');
  console.log('  --section, -s   Clause to generate (e.g. 15.7)');
  console.log('  --root, -r      Root docs folder (default: docs/ECMA262)');
  console.log('  --help, -h      Show help');
}

function requireString(obj, key) {
  const v = obj[key];
  if (typeof v !== 'string' || v.trim().length === 0) {
    throw new Error(`Missing or invalid '${key}' in JSON.`);
  }
  return v.trim();
}

function validateStatus(status) {
  const allowed = ['Untracked', 'Not Yet Supported', 'Partially Supported', 'Supported'];
  if (!allowed.includes(status)) {
    throw new Error(`Invalid status '${status}'. Allowed: ${allowed.join(', ')}`);
  }
}

function parseClause(section) {
  const m = /^([0-9]+)\.([0-9]+)$/.exec(section.trim());
  if (!m) {
    throw new Error(`Invalid --section '${section}'. Expected format like 15.7`);
  }
  return { parent: m[1], sub: m[2] };
}

function asSpecLink(url) {
  return `[tc39.es](${url})`;
}

function escapePipes(text) {
  return String(text ?? '').replace(/\|/g, '\\|');
}

function formatTableCellText(text) {
  return escapePipes(String(text ?? '')).replace(/\r?\n/g, '<br>');
}

function toPosixPath(p) {
  return String(p ?? '').replace(/\\/g, '/');
}

function formatTestScriptLink(scriptPath, mdDir, repoRootDir) {
  const raw = String(scriptPath ?? '').trim();
  if (!raw) return '';

  const posix = toPosixPath(raw);
  const isLikelyJsFile = posix.toLowerCase().endsWith('.js');
  const abs = path.isAbsolute(posix) ? posix : path.resolve(repoRootDir, posix);
  const exists = isLikelyJsFile && fs.existsSync(abs);

  // If we can't resolve it to a real repo file, fall back to code formatting.
  if (!exists) {
    return `\`${escapePipes(posix)}\``;
  }

  const rel = toPosixPath(path.relative(mdDir, abs));
  const label = path.basename(posix);
  return `[` + "`" + escapePipes(label) + "`" + `](${rel})`;
}

function formatTestScriptsCell(testScripts, mdDir, repoRootDir) {
  if (!Array.isArray(testScripts) || testScripts.length === 0) return '';
  return testScripts
    .map((s) => formatTestScriptLink(s, mdDir, repoRootDir))
    .filter((s) => s && s.length > 0)
    .join('<br>');
}

function getSpecUrlForClause(doc, clause) {
  if (!doc || typeof doc !== 'object') return '';
  if (doc.clause === clause && typeof doc.specUrl === 'string') return doc.specUrl;

  const subs = Array.isArray(doc.subclauses) ? doc.subclauses : [];
  for (const s of subs) {
    if (!s || typeof s !== 'object') continue;
    if (s.clause === clause && typeof s.specUrl === 'string') return s.specUrl;
  }

  const support = doc.support && typeof doc.support === 'object' ? doc.support : null;
  const entries = support && Array.isArray(support.entries) ? support.entries : [];
  for (const e of entries) {
    if (!e || typeof e !== 'object') continue;
    if (e.clause === clause && typeof e.specUrl === 'string' && e.specUrl.trim().length > 0) return e.specUrl;
  }

  return '';
}

function render(doc, sectionClause, mdPath, repoRootDir) {
  const clause = requireString(doc, 'clause');
  const title = requireString(doc, 'title');
  const status = requireString(doc, 'status');
  const specUrl = requireString(doc, 'specUrl');

  validateStatus(status);

  if (clause !== sectionClause) {
    throw new Error(`JSON clause '${clause}' does not match requested section '${sectionClause}'.`);
  }

  const parentClause = doc.parent && typeof doc.parent === 'object' ? requireString(doc.parent, 'clause') : sectionClause.split('.')[0];
  const parentDoc = doc.parent && typeof doc.parent === 'object' && typeof doc.parent.doc === 'string' && doc.parent.doc.trim().length > 0
    ? doc.parent.doc.trim()
    : `Section${parentClause}.md`;

  const lines = [];
  if (doc.autoGeneratedHeader === true) {
    lines.push(AUTO_GENERATED_MARKER);
    lines.push('');
  }
  lines.push(`# Section ${clause}: ${title}`);
  lines.push('');
  lines.push(`[Back to Section${parentClause}](${parentDoc}) | [Back to Index](../Index.md)`);
  lines.push('');

  if (doc.intro && typeof doc.intro === 'string' && doc.intro.trim().length > 0) {
    lines.push(doc.intro.trim());
    lines.push('');
  }

  lines.push('| Clause | Title | Status | Link |');
  lines.push('|---:|---|---|---|');
  lines.push(`| ${clause} | ${title} | ${status} | ${asSpecLink(specUrl)} |`);
  lines.push('');

  const subs = Array.isArray(doc.subclauses) ? doc.subclauses : [];
  if (subs.length > 0) {
    lines.push('## Subclauses');
    lines.push('');
    lines.push('| Clause | Title | Status | Spec |');
    lines.push('|---:|---|---|---|');

    for (const s of subs) {
      if (!s || typeof s !== 'object') continue;
      const sc = requireString(s, 'clause');
      const st = requireString(s, 'title');
      const ss = requireString(s, 'status');
      const su = requireString(s, 'specUrl');
      validateStatus(ss);
      lines.push(`| ${sc} | ${st} | ${ss} | ${asSpecLink(su)} |`);
    }

    lines.push('');
  }

  const support = doc.support && typeof doc.support === 'object' ? doc.support : null;
  const supportEntries = support && Array.isArray(support.entries) ? support.entries : [];
  if (supportEntries.length > 0) {
    const mdDir = path.dirname(mdPath);
    const entriesByClause = new Map();
    for (const e of supportEntries) {
      if (!e || typeof e !== 'object') continue;
      const ec = requireString(e, 'clause');
      const ef = requireString(e, 'feature');
      const es = requireString(e, 'status');
      validateStatus(es);
      const list = entriesByClause.get(ec) || [];
      list.push(e);
      entriesByClause.set(ec, list);
    }

    const sortedClauses = [...entriesByClause.keys()].sort((a, b) => a.localeCompare(b, undefined, { numeric: true }));

    lines.push('## Support');
    lines.push('');
    lines.push('Feature-level support tracking with test script references.');
    lines.push('');

    for (const c of sortedClauses) {
      const url = getSpecUrlForClause(doc, c);
      lines.push(`### ${c}${url ? ` (${asSpecLink(url)})` : ''}`);
      lines.push('');
      lines.push('| Feature name | Status | Test scripts | Notes |');
      lines.push('|---|---|---|---|');

      const list = entriesByClause.get(c) || [];
      list.sort((a, b) => String(a.feature || '').localeCompare(String(b.feature || ''), undefined, { numeric: true }));
      for (const e of list) {
        const feature = requireString(e, 'feature');
        const es = requireString(e, 'status');
        validateStatus(es);
        const scripts = formatTestScriptsCell(e.testScripts, mdDir, repoRootDir);
        const notes = formatTableCellText(e.notes || '');
        lines.push(`| ${formatTableCellText(feature)} | ${formatTableCellText(es)} | ${scripts} | ${notes} |`);
      }

      lines.push('');
    }
  }

  const ref = doc.reference && typeof doc.reference === 'object' ? doc.reference : null;
  if (ref && typeof ref.mode === 'string') {
    const mode = ref.mode;

    if (mode === 'stub') {
      lines.push('## Reference: Converted Spec Text');
      lines.push('');
      const note = typeof ref.note === 'string' && ref.note.trim().length > 0
        ? ref.note.trim()
        : '_Intentionally not included here. Use the tc39.es links above as the normative reference._';
      lines.push(note);
      lines.push('');
    } else if (mode === 'embedded') {
      const convertedFromHtml = requireString(ref, 'convertedFromHtml');
      const specMarkdownPath = requireString(ref, 'specMarkdownPath');
      const embeddedPath = path.resolve(__dirname, '..', '..', specMarkdownPath);
      if (!fs.existsSync(embeddedPath)) {
        throw new Error(`reference.specMarkdownPath not found: ${specMarkdownPath}`);
      }

      lines.push('## Reference: Converted Spec Text');
      lines.push('');
      lines.push(`_Converted from \`${convertedFromHtml}\` via \`scripts/ECMA262/convertEcmaExtractHtmlToMarkdown.js\`._`);
      lines.push('');
      lines.push('<details>');
      const summary =
        typeof ref.detailsSummary === 'string' && ref.detailsSummary.trim().length > 0
          ? ref.detailsSummary.trim()
          : `Show converted ECMA-262 §${clause} text`;
      lines.push(`<summary>${summary}</summary>`);
      lines.push('');
      lines.push(`<!-- BEGIN SPEC EXTRACT: ${specMarkdownPath} -->`);
      lines.push('');

      const embedLines = readText(embeddedPath).split(/\r?\n/);
      while (embedLines.length > 0 && embedLines[embedLines.length - 1] === '') {
        embedLines.pop();
      }
      for (const l of embedLines) {
        lines.push(l);
      }

      lines.push('');
      lines.push('<!-- END SPEC EXTRACT -->');
      lines.push('');
      lines.push('</details>');
      lines.push('');
    }
  }

  return lines;
}

function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    usage();
    return;
  }

  if (!args.section) {
    usage();
    process.exitCode = 1;
    return;
  }

  const rootDir = path.resolve(args.root && args.root.trim().length > 0 ? args.root : DEFAULT_ROOT);
  const parsed = parseClause(args.section);

  const sectionDir = path.join(rootDir, parsed.parent);
  const jsonPath = path.join(sectionDir, `Section${parsed.parent}_${parsed.sub}.json`);
  const mdPath = path.join(sectionDir, `Section${parsed.parent}_${parsed.sub}.md`);

  if (!fs.existsSync(jsonPath)) {
    throw new Error(`JSON not found: ${jsonPath}`);
  }

  const doc = JSON.parse(readText(jsonPath));
  const repoRootDir = path.resolve(__dirname, '..', '..');
  const lines = render(doc, `${parsed.parent}.${parsed.sub}`, mdPath, repoRootDir);
  const changed = writeTextPreserveEol(mdPath, lines);

  console.log(`${changed ? 'Generated' : 'Up-to-date'} ${path.relative(path.resolve(__dirname, '..', '..'), mdPath)}`);
}

if (require.main === module) {
  try {
    main();
  } catch (err) {
    console.error(err && err.message ? err.message : String(err));
    process.exitCode = 1;
  }
}
