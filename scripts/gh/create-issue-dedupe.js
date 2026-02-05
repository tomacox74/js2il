#!/usr/bin/env node

'use strict';

const fs = require('fs');
const { spawnSync } = require('child_process');

function fail(message) {
  process.stderr.write(`${message}\n`);
  process.exit(1);
}

function parseArgs(argv) {
  const args = {
    title: null,
    bodyFile: null,
    labels: '',
    updateIfExists: false,
  };

  for (let i = 0; i < argv.length; i++) {
    const token = argv[i];

    if (token === '--update-if-exists') {
      args.updateIfExists = true;
      continue;
    }

    if (token === '--title') {
      args.title = argv[++i];
      continue;
    }

    if (token === '--body-file') {
      args.bodyFile = argv[++i];
      continue;
    }

    if (token === '--labels') {
      args.labels = argv[++i] ?? '';
      continue;
    }

    fail(`Unknown arg: ${token}`);
  }

  if (!args.title) fail('Missing required arg: --title');
  if (!args.bodyFile) fail('Missing required arg: --body-file');

  return args;
}

function runGh(ghArgs) {
  const result = spawnSync('gh', ghArgs, { encoding: 'utf8' });

  if (result.error) {
    fail(`Failed to run gh: ${result.error.message}`);
  }

  if (result.status !== 0) {
    const stderr = (result.stderr || '').trim();
    const stdout = (result.stdout || '').trim();
    fail([`gh ${ghArgs.join(' ')}`, stdout, stderr].filter(Boolean).join('\n'));
  }

  return (result.stdout || '').trim();
}

function splitLabels(labels) {
  if (!labels) return [];
  return labels
    .split(',')
    .map((s) => s.trim())
    .filter((s) => s.length > 0);
}

function escapeForGhSearchExactTitle(title) {
  // Used within quotes: in:title "<title>"
  return title.replaceAll('"', '\\"');
}

function main() {
  const args = parseArgs(process.argv.slice(2));

  if (!fs.existsSync(args.bodyFile)) {
    fail(`Body file not found: ${args.bodyFile}`);
  }

  const labels = splitLabels(args.labels);

  const search = `in:title "${escapeForGhSearchExactTitle(args.title)}"`;

  const listJson = runGh([
    'issue',
    'list',
    '--state',
    'open',
    '--search',
    search,
    '--json',
    'number,title,url',
    '--limit',
    '50',
  ]);

  const existing = JSON.parse(listJson);
  const match = existing.find((i) => i.title === args.title);

  if (match) {
    process.stdout.write(`Found existing open issue with exact title: #${match.number} ${match.url}\n`);

    if (args.updateIfExists) {
      const editArgs = ['issue', 'edit', String(match.number), '--body-file', args.bodyFile];
      if (labels.length > 0) {
        editArgs.push('--add-label', labels.join(','));
      }
      runGh(editArgs);
      process.stdout.write(`Updated existing issue #${match.number}.\n`);
    }

    process.exit(0);
  }

  const createArgs = ['issue', 'create', '--title', args.title, '--body-file', args.bodyFile];
  for (const label of labels) {
    createArgs.push('--label', label);
  }

  const url = runGh(createArgs);
  process.stdout.write(`${url}\n`);
}

main();
