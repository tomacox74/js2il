  /*
 * Rolls up ECMA-262 coverage statuses:
 *   1) From subsection JSON docs (e.g. docs/ECMA262/15/Section15_5.json)
 *      into the parent section hub markdown (e.g. docs/ECMA262/15/Section15.md)
 *   2) From each section hub into docs/ECMA262/Index.md
 *
 * This is intentionally lightweight and does not attempt to re-split spec text
 * or regenerate subsection docs; it only keeps hub pages + the root index in sync
 * with the per-subsection JSON status values.
 *
 * Usage:
 *   node scripts/ECMA262/rollupEcma262Statuses.js
 *   node scripts/ECMA262/rollupEcma262Statuses.js --root docs/ECMA262
 */

'use strict';

const fs = require('fs');
const path = require('path');

const DEFAULT_ROOT = path.resolve(__dirname, '..', '..', 'docs', 'ECMA262');

function parseArgs(argv) {
  const args = {
    root: '',
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];

    if (a === '--help' || a === '-h') {
      args.help = true;
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
  }

  return args;
}

function usage() {
  console.log('Roll up ECMA-262 status values from subsection JSON into Section*.md and Index.md.');
  console.log('');
  console.log('  node scripts/ECMA262/rollupEcma262Statuses.js');
  console.log('');
  console.log('Options:');
  console.log('  --root, -r   Root docs folder (default: docs/ECMA262)');
  console.log('  --help, -h   Show help');
}

function detectEol(text) {
  return text.includes('\r\n') ? '\r\n' : '\n';
}

function readLines(filePath) {
  const text = fs.readFileSync(filePath, 'utf8');
  const eol = detectEol(text);
  const lines = text.split(/\r?\n/);
  if (lines.length > 0 && lines[lines.length - 1] === '') {
    lines.pop();
  }
  return { lines, eol };
}

function writeLinesPreserveEol(filePath, lines) {
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

function findLineIndex(lines, needle) {
  for (let i = 0; i < lines.length; i++) {
    if (lines[i] === needle) return i;
  }
  return -1;
}

function parsePipeRow(line) {
  const trimmed = line.trim();
  if (!trimmed.startsWith('|')) return null;

  let parts = trimmed.split('|').map((p) => p.trim());
  if (parts.length === 0) return null;

  if (parts[0] === '') parts = parts.slice(1);
  if (parts.length > 0 && parts[parts.length - 1] === '') parts = parts.slice(0, -1);

  return parts;
}

function parseClauseTableRow(line) {
  const parts = parsePipeRow(line);
  if (!parts || parts.length < 4) return null;

  return {
    Clause: parts[0],
    Title: parts[1],
    Status: parts[2],
    Link: parts[3],
  };
}

function escapePipes(text) {
  return String(text ?? '').replace(/\|/g, '\\|');
}

function validateStatus(status) {
  const allowed = [
    'Untracked',
    'N/A (informational)',
    'Not Yet Supported',
    'Incomplete',
    'Supported with Limitations',
    'Supported',
    // Legacy
    'Partially Supported',
    'Not Supported',
  ];
  if (!allowed.includes(status)) {
    throw new Error(`Invalid status '${status}'. Allowed: ${allowed.join(', ')}`);
  }
}

function normalizeLegacyStatus(status) {
  const s = String(status ?? '').trim();
  if (s === 'Not Supported') return 'Not Yet Supported';
  if (s === 'Partially Supported') return 'Supported with Limitations';
  return s;
}

// Rollup policy:
// - If everything is Untracked -> Untracked
// - If there is a mix of supported and unsupported -> Incomplete
// - Otherwise prefer the “best summary” of what exists
function getRollupStatus(statuses) {
  const norm = (Array.isArray(statuses) ? statuses : [])
    .map((s) => String(s ?? '').trim())
    .filter((s) => s.length > 0)
    // Normalize legacy spellings.
    .map((s) => {
      if (s === 'Not Supported') return 'Not Yet Supported';
      if (s === 'Partially Supported') return 'Supported with Limitations';
      return s;
    });

  // Treat informational clauses as neutral for rollups.
  // If *everything* is informational, the rollup should be informational.
  const withoutInformational = norm.filter((s) => s !== 'N/A (informational)');
  if (norm.length > 0 && withoutInformational.length === 0) return 'N/A (informational)';

  const effective = withoutInformational;

  const hasUntracked = effective.includes('Untracked');
  const hasNotYet = effective.includes('Not Yet Supported');
  const hasIncomplete = effective.includes('Incomplete');
  const hasLimitations = effective.includes('Supported with Limitations');
  const hasSupported = effective.includes('Supported');

  // Nothing tracked at all.
  if (!hasNotYet && !hasIncomplete && !hasLimitations && !hasSupported && hasUntracked) return 'Untracked';

  // Entirely unimplemented.
  if (hasNotYet && !hasIncomplete && !hasLimitations && !hasSupported) return 'Not Yet Supported';

  // Missing core semantics anywhere in the tree.
  if (hasIncomplete) return 'Incomplete';

  // If there's a mix of implemented and not-yet-supported, treat as incomplete overall.
  if (hasSupported && hasNotYet) return 'Incomplete';
  if (hasLimitations && hasNotYet) return 'Incomplete';

  // Otherwise, we have something usable.
  if (hasLimitations) return 'Supported with Limitations';
  if (hasSupported) return 'Supported';
  if (hasNotYet) return 'Not Yet Supported';

  return 'Untracked';
}

function asSpecLink(url) {
  const u = String(url ?? '').trim();
  return u ? `[tc39.es](${u})` : '';
}

function isNumericDirent(d) {
  return d && d.isDirectory && d.isDirectory() && /^\d+$/.test(d.name);
}

function listNumericSectionDirs(rootDir) {
  return fs
    .readdirSync(rootDir, { withFileTypes: true })
    .filter(isNumericDirent)
    .map((d) => d.name)
    .sort((a, b) => parseInt(a, 10) - parseInt(b, 10));
}

function listSubsectionJsonFiles(sectionDir, sectionNumber) {
  const re = new RegExp(`^Section${sectionNumber}_(\\d+)\\.json$`);

  return fs
    .readdirSync(sectionDir, { withFileTypes: true })
    .filter((d) => d.isFile && d.isFile())
    .map((d) => d.name)
    .map((name) => {
      const m = re.exec(name);
      if (!m) return null;
      return {
        sub: parseInt(m[1], 10),
        jsonFileName: name,
        jsonPath: path.join(sectionDir, name),
        mdFileName: `Section${sectionNumber}_${m[1]}.md`,
      };
    })
    .filter((x) => x !== null)
    .sort((a, b) => a.sub - b.sub);
}

function readJson(filePath) {
  const text = fs.readFileSync(filePath, 'utf8');
  return JSON.parse(text);
}

function findExistingSectionEntry(sectionLines, sectionNumber) {
  const headerIndex = findLineIndex(sectionLines, '| Clause | Title | Status | Link |');
  if (headerIndex < 0) return null;

  for (let i = headerIndex + 2; i < sectionLines.length; i++) {
    const l = sectionLines[i];
    if (!l.trim().startsWith('|')) continue;

    const r = parseClauseTableRow(l);
    if (!r) continue;
    if (r.Clause === String(sectionNumber)) return r;
  }

  return null;
}

function updateSectionHubMarkdown(sectionDir, sectionNumber, subsectionRows) {
  const sectionPath = path.join(sectionDir, `Section${sectionNumber}.md`);
  if (!fs.existsSync(sectionPath)) return { changed: false, status: null };

  const sectionRead = readLines(sectionPath);
  const sectionLines = sectionRead.lines;

  const backIndex =
    findLineIndex(sectionLines, '[Back to Index](../Index.md)') >= 0
      ? findLineIndex(sectionLines, '[Back to Index](../Index.md)')
      : findLineIndex(sectionLines, '[Back to Index](Index.md)');

  const headerLines =
    backIndex >= 0
      ? sectionLines.slice(0, backIndex + 1)
      : sectionLines.slice(0, Math.min(5, sectionLines.length));

  const existingEntry = findExistingSectionEntry(sectionLines, sectionNumber);

  // Prefer existing title/spec link so we don't have to infer anchors.
  // If missing, fall back to whatever is in the H1 line.
  let sectionTitle = existingEntry ? existingEntry.Title : '';
  let sectionSpecLink = existingEntry ? existingEntry.Link : '';

  if (!sectionTitle) {
    const h1 = sectionLines.find((l) => l.startsWith(`# Section ${sectionNumber}:`));
    if (h1) {
      const m = new RegExp(`^# Section ${sectionNumber}:\\s*(.+)$`).exec(h1);
      if (m) sectionTitle = m[1].trim();
    }
  }

  const sectionRollupStatus = getRollupStatus(subsectionRows.map((r) => r.status));

  const out = [];
  out.push(...headerLines);
  out.push('');
  out.push('_This section is split into subsection documents for readability._');
  out.push('');
  out.push('## Section Entry');
  out.push('');
  out.push('| Clause | Title | Status | Link |');
  out.push('|---:|---|---|---|');
  out.push(`| ${sectionNumber} | ${escapePipes(sectionTitle)} | ${sectionRollupStatus} | ${sectionSpecLink} |`);
  out.push('');
  out.push('## Subsections');
  out.push('');
  out.push('| Subsection | Title | Status | Spec | Document |');
  out.push('|---:|---|---|---|---|');

  for (const sr of subsectionRows) {
    const doc = `[${sr.mdFileName}](${sr.mdFileName})`;
    out.push(
      `| ${sr.clause} | ${escapePipes(sr.title)} | ${sr.status} | ${asSpecLink(sr.specUrl)} | ${doc} |`
    );
  }

  const changed = writeLinesPreserveEol(sectionPath, out);
  return { changed, status: sectionRollupStatus };
}

function updateIndexMarkdown(rootDir, sectionStatusByNumber) {
  const indexPath = path.join(rootDir, 'Index.md');
  if (!fs.existsSync(indexPath)) return false;

  const indexRead = readLines(indexPath);
  const indexLines = indexRead.lines;

  const sectionsHeaderIndex = findLineIndex(indexLines, '| Section | Title | Status | Spec | Document |');
  if (sectionsHeaderIndex < 0) return false;

  let anyChange = false;

  for (let i = sectionsHeaderIndex + 2; i < indexLines.length; i++) {
    const line = indexLines[i];
    if (!line.trim().startsWith('|')) continue;

    const m = /^\|\s*(\d+)\s*\|/.exec(line);
    if (!m) continue;

    const n = m[1];
    if (!Object.prototype.hasOwnProperty.call(sectionStatusByNumber, n)) continue;

    const cells = parsePipeRow(line);
    if (!cells || cells.length < 5) continue;

    const newStatus = sectionStatusByNumber[n];
    if (cells[2] !== newStatus) {
      cells[2] = newStatus;
      indexLines[i] = `| ${cells.join(' | ')} |`;
      anyChange = true;
    }
  }

  if (!anyChange) return false;
  return writeLinesPreserveEol(indexPath, indexLines);
}

function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    usage();
    return;
  }

  const rootDir = args.root ? path.resolve(process.cwd(), args.root) : DEFAULT_ROOT;
  if (!fs.existsSync(rootDir)) {
    throw new Error(`ECMA262 docs root not found: ${rootDir}`);
  }

  const sectionDirs = listNumericSectionDirs(rootDir);

  const sectionStatusByNumber = {};
  let changedSectionCount = 0;

  for (const sectionNumber of sectionDirs) {
    const sectionDir = path.join(rootDir, sectionNumber);

    const subsectionJsonFiles = listSubsectionJsonFiles(sectionDir, sectionNumber);
    // Some sections (e.g. Section 18) have no numbered subsection documents.
    // In those cases, keep Index.md in sync with the section hub's existing status.
    if (subsectionJsonFiles.length === 0) {
      const sectionHubPath = path.join(sectionDir, `Section${sectionNumber}.md`);
      if (fs.existsSync(sectionHubPath)) {
        const sectionRead = readLines(sectionHubPath);
        const existingEntry = findExistingSectionEntry(sectionRead.lines, sectionNumber);
        if (existingEntry && existingEntry.Status) {
          const normalizedStatus = normalizeLegacyStatus(existingEntry.Status);
          validateStatus(normalizedStatus);
          sectionStatusByNumber[String(sectionNumber)] = normalizedStatus;
        }
      }
      continue;
    }

    const subsectionRows = [];

    for (const f of subsectionJsonFiles) {
      const doc = readJson(f.jsonPath);

      const clause = String(doc.clause ?? '').trim();
      const title = String(doc.title ?? '').trim();
      const status = String(doc.status ?? '').trim();
      const specUrl = String(doc.specUrl ?? '').trim();

      if (!clause) throw new Error(`Missing 'clause' in ${f.jsonPath}`);
      if (!title) throw new Error(`Missing 'title' in ${f.jsonPath}`);
      validateStatus(status);
      const normalizedStatus = normalizeLegacyStatus(status);
      validateStatus(normalizedStatus);

      subsectionRows.push({
        clause,
        title,
        status: normalizedStatus,
        specUrl,
        mdFileName: f.mdFileName,
      });
    }

    // Keep deterministic ordering: by numeric subsection (15_1, 15_2, ...)
    subsectionRows.sort((a, b) => {
      const aSub = parseInt(String(a.clause).split('.')[1] ?? '0', 10);
      const bSub = parseInt(String(b.clause).split('.')[1] ?? '0', 10);
      return aSub - bSub;
    });

    const hub = updateSectionHubMarkdown(sectionDir, sectionNumber, subsectionRows);
    if (hub.status) {
      sectionStatusByNumber[String(sectionNumber)] = hub.status;
    }
    if (hub.changed) changedSectionCount++;
  }

  const indexChanged = updateIndexMarkdown(rootDir, sectionStatusByNumber);

  console.log(
    `Rollup complete. Updated sections: ${changedSectionCount}. Index updated: ${indexChanged ? 'yes' : 'no'}.`
  );
}

if (require.main === module) {
  main();
}
