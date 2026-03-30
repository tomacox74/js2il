/*
 * Syncs docs/ECMA262/.../Section<sec>_<sub>.json subclauses with the ECMA-262 spec.
 *
 * Specifically, for each subsection JSON file (e.g. 15.2), it populates/updates
 * the `subclauses` array using the tc39.es HTML structure.
 *
 * Notes:
 * - Missing-only: never edits/removes existing subclauses; only appends new ones.
 * - Preserves existing entries (including `status`) for matching clause numbers.
 * - Sets new subclauses' status to "Untracked".
 * - Optional: with --fill-blanks, fills missing title/specUrl on existing entries.
 * - Writes only when a file would change.
 * - Defaults to dry-run; pass --write to update files.
 *
 * Usage:
 *   node scripts/ECMA262/syncEcma262SubclausesFromSpec.js            # dry-run
 *   node scripts/ECMA262/syncEcma262SubclausesFromSpec.js --write    # write changes
 *
 * Options:
 *   --root <path>     Root docs folder (default: docs/ECMA262)
 *   --section <cl>    Only process a specific subsection clause (e.g. 14.4)
 *   --write           Write changes (default: dry-run)
 *   --fill-blanks     Fill missing title/specUrl in existing subclauses
 *   --verbose         Log per-file details
 */

'use strict';

const fs = require('fs');
const path = require('path');

const DEFAULT_ROOT = path.resolve(__dirname, '..', '..', 'docs', 'ECMA262');
const SPEC_URL = 'https://tc39.es/ecma262/';

function parseArgs(argv) {
  const args = {
    root: DEFAULT_ROOT,
    section: '',
    write: false,
    fillBlanks: false,
    verbose: false,
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];

    if (a === '--help' || a === '-h') {
      args.help = true;
      continue;
    }

    if (a === '--write') {
      args.write = true;
      continue;
    }

    if (a === '--fill-blanks') {
      args.fillBlanks = true;
      continue;
    }

    if (a === '--verbose' || a === '-v') {
      args.verbose = true;
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
      const v = argv[i + 1];
      if (v) {
        args.root = path.resolve(v);
        i++;
      }
      continue;
    }

    if (a.startsWith('--root=')) {
      args.root = path.resolve(a.substring('--root='.length));
      continue;
    }
  }

  return args;
}

function usage() {
  console.log('Sync ECMA-262 subsection JSON subclauses from tc39.es.');
  console.log('');
  console.log('  node scripts/ECMA262/syncEcma262SubclausesFromSpec.js            # dry-run');
  console.log('  node scripts/ECMA262/syncEcma262SubclausesFromSpec.js --write    # write changes');
  console.log('');
  console.log('Options:');
  console.log('  --root, -r     Root docs folder (default: docs/ECMA262)');
  console.log('  --section, -s  Only process a specific subsection clause (e.g. 14.4)');
  console.log('  --write        Actually write files (default: dry-run)');
  console.log('  --fill-blanks  Fill missing title/specUrl in existing subclauses');
  console.log('  --verbose, -v  Extra per-file logging');
  console.log('  --help, -h     Show help');
}

function normalizeClauseFilter(v) {
  const s = String(v ?? '').trim();
  if (!s) return '';
  if (!/^\d+\.\d+$/.test(s)) {
    throw new Error(`Invalid --section '${s}'. Expected a subsection clause like 14.4`);
  }
  return s;
}

function walkFiles(dir) {
  const out = [];
  const entries = fs.readdirSync(dir, { withFileTypes: true });
  for (const e of entries) {
    const full = path.join(dir, e.name);
    if (e.isDirectory()) {
      out.push(...walkFiles(full));
    } else if (e.isFile()) {
      out.push(full);
    }
  }
  return out;
}

function readJson(jsonPath) {
  const txt = fs.readFileSync(jsonPath, 'utf8');
  return JSON.parse(txt);
}

function writeJsonIfChanged(jsonPath, obj) {
  const next = JSON.stringify(obj, null, 2) + '\n';
  const prev = fs.existsSync(jsonPath) ? fs.readFileSync(jsonPath, 'utf8') : null;
  if (prev === next) return false;
  fs.writeFileSync(jsonPath, next, 'utf8');
  return true;
}

function stripTags(html) {
  return html
    .replace(/<[^>]*>/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}

function decodeEntities(text) {
  // Minimal HTML entity decoding for titles.
  return text
    .replace(/&amp;/g, '&')
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'");
}

function isNumericClauseNumber(s) {
  // We only care about numeric clause numbers like 15.2.1. Do not include Annex letters.
  return /^\d+(\.\d+)+$/.test(s);
}

async function fetchSpecHtml() {
  const res = await fetch(SPEC_URL, {
    headers: {
      // Some environments serve slightly different content without a UA; set one.
      'User-Agent': 'js2il-doc-sync-script',
    },
  });
  if (!res.ok) {
    throw new Error(`Failed to fetch ${SPEC_URL}: ${res.status} ${res.statusText}`);
  }
  return await res.text();
}

function extractClausesFromSpec(html) {
  // Extract emu-clause headings in document order.
  // We look for: <emu-clause id="..."> ... <h1> ... <span class="secnum">15.2</span> Title ... </h1>
  const entries = [];

  const re = /<emu-clause\b[^>]*\bid="([^"]+)"[^>]*>[\s\S]*?<h1\b[^>]*>[\s\S]*?<span\b[^>]*class="secnum"[^>]*>([^<]+)<\/span>([\s\S]*?)<\/h1>/gi;

  let m;
  while ((m = re.exec(html)) !== null) {
    const id = m[1].trim();
    const num = m[2].trim();
    if (!isNumericClauseNumber(num)) {
      continue;
    }

    // Title is everything in the h1 after the secnum span.
    const titleHtml = m[3];
    const title = decodeEntities(stripTags(titleHtml));

    // Some headings have a leading colon or dash due to formatting. Trim common prefixes.
    const cleanedTitle = title.replace(/^[:\-\u2013\u2014\s]+/, '').trim();

    entries.push({ number: num, id, title: cleanedTitle });
  }

  return entries;
}

function buildSubclausesFor(prefixClause, clauseEntries) {
  const prefix = prefixClause + '.';
  return clauseEntries
    .filter((e) => e.number.startsWith(prefix))
    .map((e) => ({
      clause: e.number,
      title: e.title,
      specUrl: `https://tc39.es/ecma262/#${e.id}`,
    }));
}

function isNonEmptyString(x) {
  return typeof x === 'string' && x.trim().length > 0;
}

async function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    usage();
    return;
  }

  const clauseFilter = normalizeClauseFilter(args.section);

  if (!fs.existsSync(args.root) || !fs.statSync(args.root).isDirectory()) {
    console.error(`Root not found or not a directory: ${args.root}`);
    process.exitCode = 1;
    return;
  }

  if (typeof fetch !== 'function') {
    console.error('This script requires Node.js with global fetch (Node 18+).');
    process.exitCode = 1;
    return;
  }

  console.log(`Fetching spec: ${SPEC_URL}`);
  const html = await fetchSpecHtml();
  const clauseEntries = extractClausesFromSpec(html);
  console.log(`Parsed ${clauseEntries.length} clause headings from spec.`);

  const allFiles = walkFiles(args.root);
  const jsonFiles = allFiles.filter((p) => /Section\d+_\d+\.json$/.test(path.basename(p)));

  let updated = 0;
  let unchanged = 0;
  let skippedNoSpec = 0;
  let failed = 0;

  for (const jsonPath of jsonFiles) {
    try {
      const doc = readJson(jsonPath);
      const clause = typeof doc.clause === 'string' ? doc.clause.trim() : '';
      if (!/^\d+\.\d+$/.test(clause)) {
        continue;
      }

      if (clauseFilter && clause !== clauseFilter) {
        continue;
      }

      const existingSubsRaw = Array.isArray(doc.subclauses) ? doc.subclauses : [];
      const existingClauseSet = new Set(
        existingSubsRaw
          .filter((x) => x && typeof x === 'object' && typeof x.clause === 'string')
          .map((x) => x.clause.trim())
          .filter((x) => x.length > 0),
      );

      const specSubs = buildSubclausesFor(clause, clauseEntries);
      if (specSubs.length === 0) {
        skippedNoSpec++;
        if (args.verbose) {
          console.warn(`skip (no spec matches): ${path.relative(process.cwd(), jsonPath)} (${clause})`);
        }
        continue;
      }

      let changed = false;

      // Missing-only append of brand-new spec clauses.
      const toAppend = specSubs.filter((s) => !existingClauseSet.has(s.clause));
      if (toAppend.length > 0) {
        if (!Array.isArray(doc.subclauses)) doc.subclauses = [];
        for (const s of toAppend) {
          doc.subclauses.push({
            clause: s.clause,
            title: s.title,
            status: 'Untracked',
            specUrl: s.specUrl,
          });
        }
        changed = true;
      }

      // Optional: fill blanks on existing entries (do not overwrite non-empty values).
      if (args.fillBlanks && Array.isArray(doc.subclauses)) {
        const specByClause = new Map(specSubs.map((s) => [s.clause, s]));

        for (const item of doc.subclauses) {
          if (!item || typeof item !== 'object') continue;
          if (!isNonEmptyString(item.clause)) continue;

          const key = item.clause.trim();
          const spec = specByClause.get(key);
          if (!spec) continue;

          if (!isNonEmptyString(item.title) && isNonEmptyString(spec.title)) {
            item.title = spec.title;
            changed = true;
          }

          if (!isNonEmptyString(item.specUrl) && isNonEmptyString(spec.specUrl)) {
            item.specUrl = spec.specUrl;
            changed = true;
          }
        }
      }

      if (!changed) {
        unchanged++;
        continue;
      }

      const rel = path.relative(process.cwd(), jsonPath);
      if (!args.write) {
        const afterCount = Array.isArray(doc.subclauses) ? doc.subclauses.length : 0;
        console.log(`would update: ${rel} (subclauses ${existingSubsRaw.length} -> ${afterCount})`);
        updated++;
        continue;
      }

      const wrote = writeJsonIfChanged(jsonPath, doc);
      if (wrote) {
        const afterCount = Array.isArray(doc.subclauses) ? doc.subclauses.length : 0;
        console.log(`updated: ${rel} (subclauses ${existingSubsRaw.length} -> ${afterCount})`);
        updated++;
      } else {
        unchanged++;
      }
    } catch (err) {
      failed++;
      console.error(`failed: ${path.relative(process.cwd(), jsonPath)}: ${err && err.message ? err.message : String(err)}`);
    }
  }

  const mode = args.write ? 'write' : 'dry-run';
  console.log('');
  console.log(`Done (${mode}). updated=${updated}, unchanged=${unchanged}, skippedNoSpec=${skippedNoSpec}, failed=${failed}`);

  if (failed > 0) {
    process.exitCode = 1;
  }
}

if (require.main === module) {
  main().catch((err) => {
    console.error(err && err.message ? err.message : String(err));
    process.exitCode = 1;
  });
}
