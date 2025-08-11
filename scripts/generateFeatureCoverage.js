// Generates ECMAScript2025_FeatureCoverage.md from ECMAScript2025_FeatureCoverage.json
const fs = require('fs');
const path = require('path');

const jsonPath = path.join(__dirname, '../docs/ECMAScript2025_FeatureCoverage.json');
const mdPath = path.join(__dirname, '../docs/ECMAScript2025_FeatureCoverage.md');

function link(title, url) {
  return url ? `[${title}](${url})` : title;
}

function escapePipes(text) {
  return String(text).replace(/\|/g, '\\|');
}

function featureRow(feature, sectionRef) {
  const status = escapePipes(feature.status || '');
  const scripts = (feature.testScripts || []).map(s => `${escapePipes(s)}`).join('<br>');
  const notes = escapePipes(feature.notes || '');
  return `| ${escapePipes(feature.feature)} | ${status} | ${scripts} | ${notes} | ${sectionRef} |`;
}

function processFeatures(features, sectionRef) {
  if (!features || !features.length) return '';
  // No number to sort by, just output as is
  let out = '\n| Feature | Status | Test Scripts | Notes | Section |\n|---|---|---|---|---|\n';
  for (const feature of features) {
    out += featureRow(feature, sectionRef) + '\n';
  }
  return out + '\n';
}

function processParagraphs(paragraphs, parentSection) {
  if (!paragraphs) return '';
  // Sort by paragraph number if present
  const sorted = [...paragraphs].sort((a, b) => String(a.paragraph || '').localeCompare(String(b.paragraph || ''), undefined, {numeric: true}));
  let out = '';
  for (const para of sorted) {
    const sectionRef = para.paragraph || parentSection || '';
    out += `\n#### ${link(para.title || para.paragraph, para.url)}\n`;
    out += processFeatures(para.features, sectionRef);
  }
  return out;
}

function processSubsections(subsections, parentSection) {
  if (!subsections) return '';
  // Sort by subsection number if present
  const sorted = [...subsections].sort((a, b) => String(a.subsection || '').localeCompare(String(b.subsection || ''), undefined, {numeric: true}));
  let out = '';
  for (const sub of sorted) {
    const sectionRef = sub.subsection || parentSection || '';
    out += `\n### ${link(sub.title || sub.subsection, sub.url)}\n`;
    out += processParagraphs(sub.paragraphs, sectionRef);
  }
  return out;
}

function processSections(sections) {
  // Sort by section number if present
  const sorted = [...sections].sort((a, b) => String(a.section || '').localeCompare(String(b.section || ''), undefined, {numeric: true}));
  let out = '';
  for (const sec of sorted) {
    const sectionRef = sec.section || '';
    out += `\n## ${link(sec.title || sec.section, sec.url)}\n`;
    out += processSubsections(sec.subsections, sectionRef);
  }
  return out;
}

function main() {
  const data = JSON.parse(fs.readFileSync(jsonPath, 'utf8'));
  let md = '# ECMAScript 2025 Feature Coverage\n\n';
  md += '[ECMAScript® 2025 Language Specification](https://tc39.es/ecma262/)\n\n';
  md += 'This file is auto-generated from ECMAScript2025_FeatureCoverage.json.\n';
  md += processSections(data.sections);
  // Replace placeholder for script links
  md = md.replace(/\u001A/g, '`');
  fs.writeFileSync(mdPath, md, 'utf8');
  console.log(`Generated ${mdPath}`);
}

main();
