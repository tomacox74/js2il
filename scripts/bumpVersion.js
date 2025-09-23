#!/usr/bin/env node
/*
 * bumpVersion.js
 * Automates version bumping across CHANGELOG.md and Js2IL/Js2IL.csproj.
 * Usage examples:
 *   node scripts/bumpVersion.js patch
 *   node scripts/bumpVersion.js minor
 *   node scripts/bumpVersion.js major
 *   node scripts/bumpVersion.js 0.2.0        # explicit version
 *
 * Behavior:
 * - Reads current project version from Js2IL.csproj <Version> element.
 * - Computes new version (semantic: major.minor.patch), ignoring pre-release labels for now.
 * - Validates that CHANGELOG.md has an '## Unreleased' section.
 * - Moves content under '## Unreleased' (excluding placeholder lines like '_Nothing yet._')
 *   into a new section '## vX.Y.Z - YYYY-MM-DD' above the previous releases.
 * - If Unreleased is empty and you request a bump, it still creates an empty release section unless --skip-empty.
 * - Updates <Version> in Js2IL.csproj.
 * - Writes files only if actual changes differ.
 * - Prints next steps (commit, tag).
 */

const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');
const CSPROJ_PATH = path.join(ROOT, 'Js2IL', 'Js2IL.csproj');
const CHANGELOG_PATH = path.join(ROOT, 'CHANGELOG.md');

function readFile(p) { return fs.readFileSync(p, 'utf8'); }
function writeFile(p, c) { fs.writeFileSync(p, c, 'utf8'); }

function parseCurrentVersion(csprojText) {
  const m = csprojText.match(/<Version>([^<]+)<\/Version>/);
  if(!m) throw new Error('Could not find <Version> in Js2IL.csproj');
  return m[1].trim();
}

function incVersion(version, kind) {
  const pre = version.split('-')[0];
  const [maj, min, pat] = pre.split('.').map(n => parseInt(n, 10));
  if([maj, min, pat].some(n => Number.isNaN(n))) throw new Error('Invalid current version: ' + version);
  let M=maj, m=min, p=pat;
  switch(kind){
    case 'major': M++; m=0; p=0; break;
    case 'minor': m++; p=0; break;
    case 'patch': p++; break;
    default: throw new Error('Unknown bump kind: ' + kind);
  }
  return `${M}.${m}.${p}`;
}

function resolveTargetVersion(argVersion, current) {
  if(!argVersion) throw new Error('Provide a bump type (major|minor|patch) or explicit version');
  if(/^major|minor|patch$/.test(argVersion)) return incVersion(current, argVersion);
  if(!/^\d+\.\d+\.\d+$/.test(argVersion)) throw new Error('Explicit version must be major.minor.patch');
  return argVersion;
}

function updateCsprojVersion(text, newVersion) {
  return text.replace(/<Version>[^<]+<\/Version>/, `<Version>${newVersion}<\/Version>`);
}

function extractUnreleased(changelog) {
  const unreleasedHeaderIdx = changelog.indexOf('## Unreleased');
  if(unreleasedHeaderIdx === -1) throw new Error('CHANGELOG.md missing "## Unreleased" section');
  // Find start of next release header after Unreleased
  const afterUnreleased = changelog.indexOf('\n', unreleasedHeaderIdx); // end of header line
  const nextReleaseMatch = changelog.slice(afterUnreleased + 1).match(/\n## v?\d+\.\d+\.\d+ /);
  let nextHeaderIdx = -1;
  if(nextReleaseMatch) {
    nextHeaderIdx = afterUnreleased + 1 + nextReleaseMatch.index + 1; // +1 for leading \n consumed in regex
  }
  const sectionEnd = nextHeaderIdx === -1 ? changelog.length : nextHeaderIdx - 1;
  const body = changelog.slice(afterUnreleased + 1, sectionEnd).trim();
  return { body, start: afterUnreleased + 1, end: sectionEnd };
}

function isPlaceholder(line){
  return /^_Nothing yet\._/i.test(line.trim());
}

function generateReleaseSection(newVersion, body) {
  const date = new Date().toISOString().slice(0,10);
  let cleaned = body.split(/\r?\n/).filter(l => l.trim().length > 0 && !isPlaceholder(l)).join('\n');
  if(!cleaned) cleaned = '\n';
  return `## v${newVersion} - ${date}\n\n${cleaned}\n`;
}

function perform() {
  const arg = process.argv[2];
  const skipEmpty = process.argv.includes('--skip-empty');
  const csprojText = readFile(CSPROJ_PATH);
  const changelogText = readFile(CHANGELOG_PATH);

  const currentVersion = parseCurrentVersion(csprojText);
  const newVersion = resolveTargetVersion(arg, currentVersion);
  if(currentVersion === newVersion) {
    console.error(`Version unchanged (${currentVersion}); aborting.`);
    process.exit(1);
  }

  const { body, start, end } = extractUnreleased(changelogText);
  const hasRealContent = body.split(/\r?\n/).some(l => l.trim().length && !isPlaceholder(l));
  if(!hasRealContent && skipEmpty) {
    console.log('Unreleased is empty and --skip-empty specified; only bumping csproj version.');
    const newCsproj = updateCsprojVersion(csprojText, newVersion);
    writeFile(CSPROJ_PATH, newCsproj);
    return;
  }

  const releaseSection = generateReleaseSection(newVersion, body);
  // Replace Unreleased body with placeholder
  const before = changelogText.slice(0, start);
  const after = changelogText.slice(end);
  const newUnreleased = '\n_Nothing yet._\n\n';
  const updatedChangelog = before + newUnreleased + releaseSection + after;
  const updatedCsproj = updateCsprojVersion(csprojText, newVersion);

  writeFile(CHANGELOG_PATH, updatedChangelog);
  writeFile(CSPROJ_PATH, updatedCsproj);

  console.log(`Bumped version: ${currentVersion} -> ${newVersion}`);
  console.log('Updated CHANGELOG.md and Js2IL.csproj');
  console.log('\nNext steps:');
  console.log(`  git add CHANGELOG.md Js2IL/Js2IL.csproj`);
  console.log(`  git commit -m "chore(release): cut ${newVersion}"`);
  console.log(`  git tag -a v${newVersion} -m "Release ${newVersion}"`);
  console.log('  git push && git push --tags');
}

try { perform(); } catch (e) { console.error('ERROR:', e.message); process.exit(1); }
