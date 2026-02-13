#!/usr/bin/env node
"use strict";

/*
 Generates docs/nodejs/NodeSupport.md from docs/nodejs/NodeSupport.json.

 DEPRECATED: This script generates the legacy monolithic NodeSupport.md.
 For new documentation, use the modular approach:
   - npm run generate:node-modules    (splits JSON, generates Index.md and individual docs)
   - npm run generate:node-index      (generates Index.md only)
   - npm run generate:node-module-docs (generates individual module markdown files only)
 
 The modular documentation is located in separate JSON/MD files per module/global.
*/
const fs = require('fs');
const path = require('path');

function readJson(p) {
  return JSON.parse(fs.readFileSync(p, 'utf8'));
}

function asArray(v) { return Array.isArray(v) ? v : (v == null ? [] : [v]); }

function fmtCode(s) { return s ? '`' + s + '`' : ''; }

function link(href, text) { return href ? `[${text || href}](${href})` : (text || ''); }

function renderHeader(ns) {
  const dt = new Date().toISOString().replace(/\..+Z$/, 'Z');
  const lines = [];
  lines.push(`# Node Support Coverage`);
  lines.push('');
  if (ns.nodeVersionTarget) lines.push(`Target: ${fmtCode(ns.nodeVersionTarget)}`);
  lines.push(`Generated: ${fmtCode(dt)}`);
  lines.push('');
  return lines.join('\n');
}

function renderImplementation(impl) {
  const parts = asArray(impl);
  if (!parts.length) return '';
  return parts.map(p => `- ${fmtCode(p)}`).join('\n');
}

function renderApisTable(apis) {
  if (!apis || !apis.length) return '';
  const rows = [];
  rows.push('| API | Kind | Status | Docs |');
  rows.push('| --- | ---- | ------ | ---- |');
  for (const api of apis) {
    rows.push(`| ${api.name || ''} | ${api.kind || ''} | ${api.status || ''} | ${api.docs ? link(api.docs, 'docs') : ''} |`);
  }
  return rows.join('\n');
}

function renderApiTests(apis) {
  const lines = [];
  for (const api of apis || []) {
    if (!api.tests || !api.tests.length) continue;
    lines.push(`- ${fmtCode(api.name || '')}`);
    for (const t of api.tests) {
      const label = t.name ? fmtCode(t.name) : '';
      const file = t.file ? ` (${fmtCode(t.file)})` : '';
      lines.push(`  - ${label}${file}`);
    }
  }
  return lines.join('\n');
}

function renderModules(ns) {
  const lines = [];
  lines.push('## Modules');
  lines.push('');
  for (const m of ns.modules || []) {
    lines.push(`### ${m.name} (status: ${m.status})`);
    if (m.docs) lines.push(`Docs: ${link(m.docs)}`);
    if (m.implementation) {
      lines.push('Implementation:');
      lines.push(renderImplementation(m.implementation));
    }
    lines.push('');
    const table = renderApisTable(m.apis || []);
    if (table) {
      lines.push(table);
      lines.push('');
    }
    const tests = renderApiTests(m.apis || []);
    if (tests) {
      lines.push('Tests:');
      lines.push(tests);
      lines.push('');
    }
  }
  return lines.join('\n');
}

function renderGlobals(ns) {
  const lines = [];
  lines.push('## Globals');
  lines.push('');
  for (const g of ns.globals || []) {
    lines.push(`### ${g.name} (status: ${g.status})`);
    if (g.docs) lines.push(`Docs: ${link(g.docs)}`);
    if (g.implementation) {
      lines.push('Implementation:');
      lines.push(renderImplementation(g.implementation));
    }
    if (g.notes) {
      lines.push('Notes:');
      lines.push(g.notes);
    }
    if (g.tests && g.tests.length) {
      lines.push('Tests:');
      for (const t of g.tests) {
        const label = t.name ? fmtCode(t.name) : '';
        const file = t.file ? ` (${fmtCode(t.file)})` : '';
        lines.push(`- ${label}${file}`);
      }
    }
    lines.push('');
  }
  return lines.join('\n');
}

function renderLimitations(ns) {
  const items = ns.limitations || [];
  if (!items.length) return '';
  const lines = [];
  lines.push('## Limitations');
  lines.push('');
  for (const it of items) lines.push(`- ${it}`);
  lines.push('');
  return lines.join('\n');
}

function main() {
  const repoRoot = path.resolve(__dirname, '..');
  const jsonPath = path.join(repoRoot, 'docs', 'nodejs', 'NodeSupport.json');
  const outPath = path.join(repoRoot, 'docs', 'nodejs', 'NodeSupport.md');
  const ns = readJson(jsonPath);

  const parts = [];
  parts.push(renderHeader(ns));
  parts.push(renderModules(ns));
  parts.push(renderGlobals(ns));
  const lim = renderLimitations(ns);
  if (lim) parts.push(lim);
  const md = parts.join('\n\n').replace(/\r?\n/g, '\n');
  fs.writeFileSync(outPath, md, 'utf8');
  console.log(`Wrote ${path.relative(repoRoot, outPath)}`);
}

main();
