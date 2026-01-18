/*
 * Port of scripts/splitEcma262SectionsIntoSubsections.ps1
 *
 * Generates/updates docs/ECMA262 subsection docs:
 * - Splits SectionN.md clause tables into SectionN_k.md files
 * - Rebuilds SectionN.md hub pages from subsection docs (idempotent)
 * - Updates Index.md section rollup status
 *
 * Status rollup precedence:
 *   Not Yet Supported > Partially Supported > Supported > Untracked
 * Legacy "Not Supported" is treated as "Not Yet Supported".
 */

'use strict';

const fs = require('fs');
const path = require('path');

const DEFAULT_ROOT = path.resolve(__dirname, '..', 'docs', 'ECMA262');
const AUTO_GENERATED_MARKER = '<!-- AUTO-GENERATED: splitEcma262SectionsIntoSubsections.ps1 -->';

function parseArgs(argv) {
  const args = {
    root: '',
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];

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

function detectEol(text) {
  return text.includes('\r\n') ? '\r\n' : '\n';
}

function readLines(filePath) {
  const text = fs.readFileSync(filePath, 'utf8');
  const eol = detectEol(text);
  // Match PowerShell Get-Content behavior: do not include a trailing empty line
  // just because the file ends with a newline.
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
    return;
  }

  fs.writeFileSync(filePath, newText, 'utf8');
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

function getRollupStatus(statuses) {
  const norm = statuses
    .map((s) => (s ?? '').trim())
    .filter((s) => s.length > 0);

  if (norm.includes('Not Yet Supported')) return 'Not Yet Supported';
  if (norm.includes('Not Supported')) return 'Not Yet Supported';
  if (norm.includes('Partially Supported')) return 'Partially Supported';
  if (norm.includes('Supported')) return 'Supported';
  return 'Untracked';
}

function getSubsectionDocTableRowCount(filePath) {
  if (!fs.existsSync(filePath)) return 0;

  const { lines } = readLines(filePath);
  const headerIndex = findLineIndex(lines, '| Clause | Title | Status | Link |');
  if (headerIndex < 0) return 0;

  let count = 0;
  for (let i = headerIndex + 2; i < lines.length; i++) {
    const l = lines[i];
    if (!l.trim().startsWith('|')) continue;

    const row = parseClauseTableRow(l);
    if (!row) continue;
    count++;
  }

  return count;
}

function shouldOverwriteSubsectionDoc(filePath) {
  if (!fs.existsSync(filePath)) return true;

  const firstChunk = fs.readFileSync(filePath, 'utf8').split(/\r?\n/).slice(0, 20);
  if (firstChunk.includes(AUTO_GENERATED_MARKER)) return true;

  // Heuristic for older auto-generated docs: allow overwrite only if the table is a stub (<= 1 data row).
  const rowCount = getSubsectionDocTableRowCount(filePath);
  return rowCount <= 1;
}

function getSectionIndex(indexPath) {
  const sectionIndex = new Map();

  if (!fs.existsSync(indexPath)) return sectionIndex;

  const { lines } = readLines(indexPath);
  const sectionsHeaderIndex = findLineIndex(lines, '| Section | Title | Status | Spec | Document |');
  if (sectionsHeaderIndex < 0) return sectionIndex;

  for (let i = sectionsHeaderIndex + 2; i < lines.length; i++) {
    const line = lines[i];
    if (!line.trim().startsWith('|')) continue;

    const m = /^\|\s*(\d+)\s*\|/.exec(line);
    if (!m) continue;

    const cells = parsePipeRow(line);
    if (!cells || cells.length < 5) continue;

    const n = cells[0];
    const title = cells[1];
    const spec = cells[3];

    sectionIndex.set(n, { Title: title, Spec: spec });
  }

  return sectionIndex;
}

function listSectionFiles(rootDir) {
  const names = fs.readdirSync(rootDir);

  // New layout: docs/ECMA262/<sectionNumber>/Section<sectionNumber>.md
  const sections = [];
  for (const n of names) {
    if (!/^\d+$/.test(n)) continue;

    const sectionDir = path.join(rootDir, n);
    if (!fs.existsSync(sectionDir) || !fs.statSync(sectionDir).isDirectory()) continue;

    const sectionFile = path.join(sectionDir, `Section${n}.md`);
    if (!fs.existsSync(sectionFile)) continue;

    sections.push({ section: parseInt(n, 10), filePath: sectionFile });
  }

  sections.sort((a, b) => a.section - b.section);
  return sections.map((s) => s.filePath);
}

function listSubsectionFiles(sectionDir, sectionNumber) {
  const names = fs.readdirSync(sectionDir);

  const matches = [];
  for (const n of names) {
    const m = new RegExp(`^Section${sectionNumber}_(\\d+)\\.md$`).exec(n);
    if (!m) continue;
    matches.push({ fileName: n, sub: parseInt(m[1], 10) });
  }

  matches.sort((a, b) => a.sub - b.sub);
  return matches;
}

function main() {
  const args = parseArgs(process.argv);
  const rootDir = path.resolve(args.root && args.root.trim().length > 0 ? args.root : DEFAULT_ROOT);

  const indexPath = path.join(rootDir, 'Index.md');
  const sectionIndex = getSectionIndex(indexPath);

  const sectionFiles = listSectionFiles(rootDir);
  const allSectionRollups = new Map();

  for (const sectionPath of sectionFiles) {
    const baseName = path.basename(sectionPath);
    const mSection = /^Section(\d+)\.md$/.exec(baseName);
    if (!mSection) continue;

    const sectionNumber = parseInt(mSection[1], 10);
    const sectionDir = path.dirname(sectionPath);

    // If this section still has the full clause table, generate subsection docs.
    const sectionRead = readLines(sectionPath);
    const tableHeaderIndex = findLineIndex(sectionRead.lines, '| Clause | Title | Status | Link |');

    if (tableHeaderIndex >= 0) {
      const dataLines = [];
      for (let i = tableHeaderIndex + 2; i < sectionRead.lines.length; i++) {
        const l = sectionRead.lines[i];
        if (l.trim().startsWith('|')) dataLines.push(l);
      }

      const rows = [];
      for (const dl of dataLines) {
        const r = parseClauseTableRow(dl);
        if (r) rows.push(r);
      }

      const childRe = new RegExp(`^${sectionNumber}\\.\\d+`);
      const hasChildClauses = rows.some((r) => childRe.test(r.Clause));

      if (hasChildClauses) {
        const subKeyToRows = new Map();

        const groupRe = new RegExp(`^${sectionNumber}\\.(\\d+)(\\.|$)`);
        for (const r of rows) {
          const m = groupRe.exec(r.Clause);
          if (!m) continue;

          const sub = m[1];
          if (!subKeyToRows.has(sub)) subKeyToRows.set(sub, []);
          subKeyToRows.get(sub).push(r);
        }

        const subsSorted = Array.from(subKeyToRows.keys())
          .map((s) => parseInt(s, 10))
          .sort((a, b) => a - b)
          .map((n) => String(n));

        for (const sub of subsSorted) {
          const subClause = `${sectionNumber}.${sub}`;
          const subRows = rows.filter((r) => r.Clause === subClause || r.Clause.startsWith(`${subClause}.`));
          if (subRows.length === 0) continue;

          const subTitleRow = subRows.find((r) => r.Clause === subClause);
          const subTitle = subTitleRow ? subTitleRow.Title : subRows[0].Title;

          const subPath = path.join(sectionDir, `Section${sectionNumber}_${sub}.md`);
          if (!shouldOverwriteSubsectionDoc(subPath)) continue;

          const content = [];
          content.push(AUTO_GENERATED_MARKER);
          content.push('');
          content.push(`# Section ${subClause}: ${subTitle}`);
          content.push('');
          content.push(`[Back to Section${sectionNumber}](Section${sectionNumber}.md) | [Back to Index](../Index.md)`);
          content.push('');
          content.push('| Clause | Title | Status | Link |');
          content.push('|---:|---|---|---|');

          for (const sr of subRows) {
            content.push(`| ${sr.Clause} | ${sr.Title} | ${sr.Status} | ${sr.Link} |`);
          }

          writeLinesPreserveEol(subPath, content);
        }
      }
    }

    // If we have subsection docs, rebuild SectionN.md from them (idempotent)
    const subFiles = listSubsectionFiles(sectionDir, sectionNumber);
    if (subFiles.length === 0) continue;

    const sectionLines = readLines(sectionPath).lines;
    const backIndex =
      findLineIndex(sectionLines, '[Back to Index](../Index.md)') >= 0
        ? findLineIndex(sectionLines, '[Back to Index](../Index.md)')
        : findLineIndex(sectionLines, '[Back to Index](Index.md)');
    const headerLines =
      backIndex >= 0
        ? sectionLines.slice(0, backIndex + 1)
        : sectionLines.slice(0, Math.min(5, sectionLines.length));

    const sectionMeta = sectionIndex.get(String(sectionNumber));
    const sectionTitle = sectionMeta ? sectionMeta.Title : '';
    const sectionSpec = sectionMeta ? sectionMeta.Spec : '';

    const subRowsForIndex = [];

    for (const sf of subFiles) {
      const subClause = `${sectionNumber}.${sf.sub}`;
      const subPath = path.join(sectionDir, sf.fileName);

      const subLines = readLines(subPath).lines;
      const subHeaderIndex = findLineIndex(subLines, '| Clause | Title | Status | Link |');
      if (subHeaderIndex < 0) continue;

      let found = null;
      for (let i = subHeaderIndex + 2; i < subLines.length; i++) {
        const l = subLines[i];
        if (!l.trim().startsWith('|')) continue;

        const r = parseClauseTableRow(l);
        if (!r) continue;
        if (r.Clause === subClause) {
          found = r;
          break;
        }
      }

      if (found) {
        subRowsForIndex.push({
          Sub: sf.sub,
          Clause: subClause,
          Title: found.Title,
          Status: found.Status,
          Link: found.Link,
          File: sf.fileName,
        });
      }
    }

    subRowsForIndex.sort((a, b) => a.Sub - b.Sub);
    const sectionRollupStatus = getRollupStatus(subRowsForIndex.map((r) => r.Status));
    allSectionRollups.set(String(sectionNumber), sectionRollupStatus);

    const out = [];
    out.push(...headerLines);
    out.push('');
    out.push('_This section is split into subsection documents for readability._');
    out.push('');
    out.push('## Section Entry');
    out.push('');
    out.push('| Clause | Title | Status | Link |');
    out.push('|---:|---|---|---|');
    out.push(`| ${sectionNumber} | ${sectionTitle} | ${sectionRollupStatus} | ${sectionSpec} |`);
    out.push('');
    out.push('## Subsections');
    out.push('');
    out.push('| Subsection | Title | Status | Spec | Document |');
    out.push('|---:|---|---|---|---|');

    for (const sr of subRowsForIndex) {
      const doc = `[${sr.File}](${sr.File})`;
      out.push(`| ${sr.Clause} | ${sr.Title} | ${sr.Status} | ${sr.Link} | ${doc} |`);
    }

    writeLinesPreserveEol(sectionPath, out);
  }

  // Update Index.md section status column based on new rollups
  if (fs.existsSync(indexPath)) {
    const indexRead = readLines(indexPath);
    const indexLines = indexRead.lines;

    const sectionsHeaderIndex = findLineIndex(indexLines, '| Section | Title | Status | Spec | Document |');
    if (sectionsHeaderIndex >= 0) {
      for (let i = sectionsHeaderIndex + 2; i < indexLines.length; i++) {
        const line = indexLines[i];
        if (!line.trim().startsWith('|')) continue;

        const m = /^\|\s*(\d+)\s*\|/.exec(line);
        if (!m) continue;

        const n = m[1];
        if (!allSectionRollups.has(n)) continue;

        const cells = parsePipeRow(line);
        if (!cells || cells.length < 5) continue;

        cells[2] = allSectionRollups.get(n);
        indexLines[i] = `| ${cells.join(' | ')} |`;
      }

      writeLinesPreserveEol(indexPath, indexLines);
    }
  }

  console.log('Split complete: subsection docs generated and indexes updated.');
}

if (require.main === module) {
  main();
}
