#!/usr/bin/env node
"use strict";

/**
 * Generates docs/nodejs/Index.md from individual module/global JSON files.
 * 
 * Usage:
 *   node scripts/generateNodeIndex.js
 */

const fs = require('fs');
const path = require('path');

function sanitizeFileName(name) {
  return name.replace(/[\/\.]/g, '_');
}

function link(href, text) {
  return href ? `[${text || href}](${href})` : (text || '');
}

function sortByName(a, b) {
  return a.name.localeCompare(b.name);
}

function main() {
  const repoRoot = path.resolve(__dirname, '..');
  const nodejsDir = path.join(repoRoot, 'docs', 'nodejs');
  
  // Read all JSON files (except schemas and NodeSupport.json)
  const allFiles = fs.readdirSync(nodejsDir);
  const jsonFiles = allFiles
    .filter(f => f.endsWith('.json'))
    .filter(f => f !== 'NodeSupport.json' && 
                 f !== 'NodeSupport.schema.json' && 
                 f !== 'ModuleDoc.schema.json' &&
                 f !== 'NodeLimitations.json')
    .map(f => path.join(nodejsDir, f));
  
  // Parse all module/global docs
  const modules = [];
  const globals = [];
  let nodeVersionTarget = '';
  
  for (const jsonPath of jsonFiles) {
    try {
      const doc = JSON.parse(fs.readFileSync(jsonPath, 'utf8'));
      if (!nodeVersionTarget && doc.nodeVersionTarget) {
        nodeVersionTarget = doc.nodeVersionTarget;
      }
      
      if (doc.type === 'module') {
        modules.push(doc);
      } else if (doc.type === 'global') {
        globals.push(doc);
      }
    } catch (err) {
      console.error(`Warning: Failed to parse ${path.basename(jsonPath)}: ${err.message}`);
    }
  }
  
  // Sort by name
  modules.sort(sortByName);
  globals.sort(sortByName);
  
  // Read limitations
  let limitations = [];
  const limitationsPath = path.join(nodejsDir, 'NodeLimitations.json');
  if (fs.existsSync(limitationsPath)) {
    try {
      const limitationsDoc = JSON.parse(fs.readFileSync(limitationsPath, 'utf8'));
      limitations = limitationsDoc.limitations || [];
    } catch (err) {
      console.error(`Warning: Failed to parse NodeLimitations.json: ${err.message}`);
    }
  }
  
  // Generate Index.md
  const lines = [];
  
  lines.push('<!-- AUTO-GENERATED: generateNodeIndex.js -->');
  lines.push('');
  lines.push('# Node.js Support Coverage');
  lines.push('');
  
  if (nodeVersionTarget) {
    lines.push(`**Target Node.js Version:** \`${nodeVersionTarget}\``);
    lines.push('');
  }
  
  const dt = new Date().toISOString().replace(/\..+Z$/, 'Z');
  lines.push(`**Generated:** \`${dt}\``);
  lines.push('');
  
  // Summary
  lines.push('## Summary');
  lines.push('');
  lines.push(`- **Modules:** ${modules.length}`);
  lines.push(`- **Globals:** ${globals.length}`);
  
  const supportedModules = modules.filter(m => m.status === 'supported').length;
  const partialModules = modules.filter(m => m.status === 'partial').length;
  const supportedGlobals = globals.filter(g => g.status === 'supported').length;
  const partialGlobals = globals.filter(g => g.status === 'partial').length;
  
  lines.push(`  - Supported: ${supportedModules + supportedGlobals}`);
  lines.push(`  - Partial: ${partialModules + partialGlobals}`);
  lines.push('');
  
  // Modules table
  if (modules.length > 0) {
    lines.push('## Modules');
    lines.push('');
    lines.push('| Module | Status | Documentation |');
    lines.push('| --- | --- | --- |');
    
    for (const mod of modules) {
      const fileName = sanitizeFileName(mod.name) + '.md';
      const name = link(fileName, mod.name);
      const docsLink = mod.docsUrl ? link(mod.docsUrl, 'Node.js') : '';
      lines.push(`| ${name} | ${mod.status} | ${docsLink} |`);
    }
    lines.push('');
  }
  
  // Globals table
  if (globals.length > 0) {
    lines.push('## Globals');
    lines.push('');
    lines.push('| Global | Status | Documentation |');
    lines.push('| --- | --- | --- |');
    
    for (const glob of globals) {
      const fileName = sanitizeFileName(glob.name) + '.md';
      const name = link(fileName, glob.name);
      const docsLink = glob.docsUrl ? link(glob.docsUrl, 'Node.js') : '';
      lines.push(`| ${name} | ${glob.status} | ${docsLink} |`);
    }
    lines.push('');
  }
  
  // Limitations
  if (limitations.length > 0) {
    lines.push('## Limitations');
    lines.push('');
    for (const limitation of limitations) {
      lines.push(`- ${limitation}`);
    }
    lines.push('');
  }
  
  // Write Index.md
  const indexPath = path.join(nodejsDir, 'Index.md');
  const markdown = lines.join('\n');
  fs.writeFileSync(indexPath, markdown, 'utf8');
  
  console.log(`Generated ${path.relative(repoRoot, indexPath)}`);
  console.log(`  - Modules: ${modules.length}`);
  console.log(`  - Globals: ${globals.length}`);
  console.log(`  - Limitations: ${limitations.length}`);
}

main();
