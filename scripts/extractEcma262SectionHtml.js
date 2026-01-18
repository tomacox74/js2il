/*
 * Extract a section's HTML from a locally saved ECMA-262 multipage HTML file.
 *
 * Why local input?
 * - This avoids baking "download the spec" behavior into the repo tooling.
 * - You can point it at an HTML file you already have (e.g. saved from tc39.es).
 *
 * Usage:
 *   node scripts/extractEcma262SectionHtml.js --section 27.3 --in control-abstraction-objects.html --out Section27_3.html
 *   node scripts/extractEcma262SectionHtml.js --section 27.3 --url https://tc39.es/ecma262/multipage/control-abstraction-objects.html --out Section27_3.html
 *
 * Options:
 *   --section, -s   Section number to extract (e.g. 27.3)
 *   --in, -i        Input HTML file path
 *   --url, -u       Fetch input HTML from URL instead of --in
 *   --auto          Auto-discover the correct multipage URL from the multipage index (implies network fetch)
 *   --index-url     Multipage index URL used by --auto (default: https://tc39.es/ecma262/multipage/)
 *   --out, -o       Output file path
 *   --id            Explicit element id to extract (e.g. sec-generatorfunction-objects)
 *   --wrap          Wrap output as standalone HTML (default)
 *   --no-wrap       Output only the extracted element HTML
 *   --base          Optional <base href="..."> to inject when wrapping
 *   --help, -h      Show help
 */

'use strict';

const fs = require('fs');
const path = require('path');
const https = require('https');

function parseArgs(argv) {
  const args = {
    section: '',
    inFile: '',
    url: '',
    auto: false,
    indexUrl: 'https://tc39.es/ecma262/multipage/',
    outFile: '',
    id: '',
    wrap: true,
    baseHref: '',
    help: false,
  };

  const positionals = [];

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

    if (a === '--in' || a === '-i') {
      args.inFile = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--in=')) {
      args.inFile = a.substring('--in='.length);
      continue;
    }

    if (a === '--url' || a === '-u') {
      args.url = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--url=')) {
      args.url = a.substring('--url='.length);
      continue;
    }

    if (a === '--auto') {
      args.auto = true;
      continue;
    }

    if (a === '--index-url') {
      args.indexUrl = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--index-url=')) {
      args.indexUrl = a.substring('--index-url='.length);
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

    if (a === '--id') {
      args.id = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--id=')) {
      args.id = a.substring('--id='.length);
      continue;
    }

    if (a === '--wrap') {
      args.wrap = true;
      continue;
    }

    if (a === '--no-wrap') {
      args.wrap = false;
      continue;
    }

    if (a === '--base') {
      args.baseHref = argv[i + 1] || '';
      i++;
      continue;
    }

    if (a.startsWith('--base=')) {
      args.baseHref = a.substring('--base='.length);
      continue;
    }

    positionals.push(a);
  }

  // Positional fallback:
  //   node script.js 27.3 input.html output.html
  if (!args.section && positionals.length >= 1) args.section = positionals[0];
  if (!args.inFile && !args.url && positionals.length >= 2) args.inFile = positionals[1];
  if (!args.outFile && positionals.length >= 3) args.outFile = positionals[2];

  return args;
}

function fetchTextWithHttps(urlString, maxRedirects = 5) {
  return new Promise((resolve, reject) => {
    let urlObj;
    try {
      urlObj = new URL(urlString);
    } catch {
      reject(new Error(`Invalid URL: ${urlString}`));
      return;
    }

    if (urlObj.protocol !== 'https:') {
      reject(new Error(`Only https:// URLs are supported by the https fallback. Got: ${urlString}`));
      return;
    }

    const req = https.request(
      urlObj,
      {
        method: 'GET',
        headers: {
          // Avoid needing to implement gzip/br decoding.
          'Accept-Encoding': 'identity',
          'User-Agent': 'js2il-docs-script',
        },
      },
      (res) => {
        const status = res.statusCode || 0;
        const location = res.headers.location;

        if (status >= 300 && status < 400 && location) {
          if (maxRedirects <= 0) {
            res.resume();
            reject(new Error(`Too many redirects fetching ${urlString}`));
            return;
          }

          const nextUrl = new URL(location, urlObj).toString();
          res.resume();
          fetchTextWithHttps(nextUrl, maxRedirects - 1).then(resolve, reject);
          return;
        }

        if (status < 200 || status >= 300) {
          res.resume();
          reject(new Error(`HTTP ${status} fetching ${urlString}`));
          return;
        }

        res.setEncoding('utf8');
        let data = '';
        res.on('data', (chunk) => {
          data += chunk;
        });
        res.on('end', () => resolve(data));
      }
    );

    req.on('error', reject);
    req.end();
  });
}

async function fetchText(urlString) {
  // Prefer the built-in fetch (Node 18+). Fall back to https for older Node.
  if (typeof fetch === 'function') {
    const res = await fetch(urlString, {
      redirect: 'follow',
      headers: {
        'User-Agent': 'js2il-docs-script',
      },
    });

    if (!res.ok) {
      throw new Error(`HTTP ${res.status} fetching ${urlString}`);
    }

    return await res.text();
  }

  return await fetchTextWithHttps(urlString);
}

function detectEol(text) {
  return text.includes('\r\n') ? '\r\n' : '\n';
}

function escapeRegExp(text) {
  return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function writeTextPreserveEol(filePath, text) {
  let eol = '\n';
  let existingText = null;

  if (fs.existsSync(filePath)) {
    existingText = fs.readFileSync(filePath, 'utf8');
    eol = detectEol(existingText);
  }

  // Normalize line endings in output to preserve existing file style.
  const normalized = text.replace(/\r\n|\n/g, eol);

  if (existingText !== null && existingText === normalized) {
    return;
  }

  fs.mkdirSync(path.dirname(filePath), { recursive: true });
  fs.writeFileSync(filePath, normalized, 'utf8');
}

function findTagNameAt(html, tagStart) {
  if (tagStart < 0 || html[tagStart] !== '<') return '';

  const afterLt = tagStart + 1;
  const slash = html[afterLt] === '/';
  const nameStart = slash ? afterLt + 1 : afterLt;

  let i = nameStart;
  while (i < html.length) {
    const ch = html[i];
    if (ch === ' ' || ch === '\t' || ch === '\r' || ch === '\n' || ch === '>' || ch === '/') {
      break;
    }
    i++;
  }

  return html.substring(nameStart, i);
}

function extractElementById(html, elementId) {
  const idDouble = `id="${elementId}"`;
  const idSingle = `id='${elementId}'`;

  let idIndex = html.indexOf(idDouble);
  if (idIndex < 0) idIndex = html.indexOf(idSingle);
  if (idIndex < 0) {
    throw new Error(`Could not find id '${elementId}' in input HTML.`);
  }

  const tagStart = html.lastIndexOf('<', idIndex);
  if (tagStart < 0) {
    throw new Error(`Could not locate tag start for id '${elementId}'.`);
  }

  const tagName = findTagNameAt(html, tagStart);
  if (!tagName) {
    throw new Error(`Could not determine tag name for id '${elementId}'.`);
  }

  // Walk matching open/close tags of the same tag name.
  const tagRe = new RegExp(`<\\/?${escapeRegExp(tagName)}\\b[^>]*>`, 'gi');
  tagRe.lastIndex = tagStart;

  let depth = 0;
  let startIndex = -1;

  while (true) {
    const m = tagRe.exec(html);
    if (!m) break;

    const token = m[0];

    if (startIndex < 0) {
      startIndex = m.index;
    }

    const isClose = token.startsWith(`</`);
    if (isClose) {
      depth--;
      if (depth === 0) {
        const endIndex = tagRe.lastIndex;
        return {
          tagName,
          html: html.substring(startIndex, endIndex),
        };
      }
    } else {
      depth++;
    }
  }

  throw new Error(`Could not find a matching closing </${tagName}> for id '${elementId}'.`);
}

function resolveSectionLinkFromMultipageIndexHtml(indexHtml, section) {
  const sectionEsc = escapeRegExp(section);

  // The multipage index contains TOC links like:
  //   <a href="control-abstraction-objects.html#sec-generatorfunction-objects"> <span class="secnum">27.3</span> ...</a>
  // Constrain match to within a single <a>..</a> to avoid spanning.
  const re = new RegExp(
    `<a\\b[^>]*href=(?:\"([^\"]+)\"|'([^']+)')[^>]*>(?:(?!<\\/a>)[\\s\\S]){0,4000}?<span\\b[^>]*class=(?:\"secnum\"|'secnum')[^>]*>\\s*${sectionEsc}\\s*<\\/span>(?:(?!<\\/a>)[\\s\\S]){0,4000}?<\\/a>`,
    'i'
  );

  const m = re.exec(indexHtml);
  const href = (m && (m[1] || m[2])) || '';
  if (!href) return null;

  const hashIndex = href.indexOf('#');
  const filePart = hashIndex >= 0 ? href.substring(0, hashIndex) : href;
  const fragment = hashIndex >= 0 ? href.substring(hashIndex + 1) : '';

  return {
    href,
    filePart,
    fragment,
  };
}

function resolveSectionIdFromHtml(html, section) {
  const sectionEsc = escapeRegExp(section);

  // Most reliable across tc39.es multipage output: the Table of Contents contains an <a href="...#<id>">
  // with a <span class="secnum">27.3</span>.
  {
    const re = new RegExp(
      `<a\\b[^>]*href=(?:\"[^\"]*#([^\"#]+)\"|'[^']*#([^'#]+)')[^>]*>(?:(?!<\\/a>)[\\s\\S]){0,4000}?<span\\b[^>]*class=(?:\"secnum\"|'secnum')[^>]*>\\s*${sectionEsc}\\s*<\\/span>(?:(?!<\\/a>)[\\s\\S]){0,4000}?<\\/a>`,
      'i'
    );
    const m = re.exec(html);
    if (m) return m[1] || m[2] || '';
  }

  // Most reliable (ecmarkup output): <emu-clause id="sec-..."> ... <span class="secnum">27.3</span>
  // The section number is often not a raw text prefix of the heading; it is typically wrapped in a <span class="secnum">.
  {
    const re = new RegExp(
      `<emu-clause\\b[^>]*id=(?:\"([^\"]+)\"|'([^']+)')[^>]*>[\\s\\S]{0,8000}?<h[1-6]\\b[^>]*>[\\s\\S]{0,1200}?<span\\b[^>]*class=(?:\"secnum\"|'secnum')[^>]*>\\s*${sectionEsc}\\s*<\\/span>[\\s\\S]{0,1200}?<\\/h[1-6]>`,
      'i'
    );
    const m = re.exec(html);
    if (m) return m[1] || m[2] || '';
  }

  // Fallback: id on a heading element
  {
    // Handle either raw text "27.3" or <span class="secnum">27.3</span> within the heading.
    const re = new RegExp(
      `<h[1-6]\\b[^>]*id=(?:\"([^\"]+)\"|'([^']+)')[^>]*>[\\s\\S]{0,1200}?(?:\\s*${sectionEsc}\\b|<span\\b[^>]*class=(?:\"secnum\"|'secnum')[^>]*>\\s*${sectionEsc}\\s*<\\/span>)`,
      'i'
    );
    const m = re.exec(html);
    if (m) return m[1] || m[2] || '';
  }

  return '';
}

function wrapAsStandaloneHtml(extractedHtml, options) {
  const title = options.title || 'ECMA-262 Section Extract';
  const baseHref = options.baseHref || '';

  const baseTag = baseHref ? `  <base href="${baseHref}">\n` : '';

  // Minimal CSS to make ecmarkup custom elements readable without pulling in tc39 styles.
  const css = `  <style>
    body { font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin: 2rem; line-height: 1.35; }
    emu-clause, emu-intro, emu-annex, emu-section, emu-subsection, emu-table, emu-figure, emu-note, emu-example, emu-alg, emu-grammar, emu-prodref, emu-xref { display: block; }
    emu-note { border-left: 4px solid #ccc; padding-left: 1rem; margin-left: 0; }
    table { border-collapse: collapse; }
    th, td { border: 1px solid #ddd; padding: 0.25rem 0.5rem; vertical-align: top; }
    pre { background: #f6f8fa; padding: 0.75rem; overflow: auto; }
    code { background: #f6f8fa; padding: 0 0.25rem; border-radius: 3px; }
  </style>\n`;

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
${baseTag}${css}  <title>${title}</title>
</head>
<body>
${extractedHtml}
</body>
</html>
`;
}

function printHelp() {
  console.log('Extract a section\'s HTML from a locally saved ECMA-262 multipage HTML file.');
  console.log('You can also fetch the input HTML from the web using Node\'s built-in fetch/https.');
  console.log('');
  console.log('Usage:');
  console.log('  node scripts/extractEcma262SectionHtml.js --section 27.3 --in <input.html> --out <output.html>');
  console.log('  node scripts/extractEcma262SectionHtml.js --section 27.3 --url <https://...> --out <output.html>');
  console.log('  node scripts/extractEcma262SectionHtml.js --section 27.3 --auto --out <output.html>');
  console.log('');
  console.log('Options:');
  console.log('  --section, -s   Section number to extract (e.g. 27.3)');
  console.log('  --in, -i        Input HTML file path');
  console.log('  --url, -u       Fetch input HTML from URL instead of --in');
  console.log('  --auto          Auto-discover the correct multipage URL from the multipage index');
  console.log('  --index-url     Multipage index URL used by --auto (default: https://tc39.es/ecma262/multipage/)');
  console.log('  --out, -o       Output file path');
  console.log('  --id            Explicit element id to extract (e.g. sec-generatorfunction-objects)');
  console.log('  --wrap          Wrap output as standalone HTML (default)');
  console.log('  --no-wrap       Output only the extracted element HTML');
  console.log('  --base          Optional <base href="..."> to inject when wrapping');
  console.log('  --help, -h      Show help');
}

async function main() {
  const args = parseArgs(process.argv);

  if (args.help) {
    printHelp();
    return;
  }

  if (!args.section) {
    throw new Error('Missing required --section (e.g. 27.3).');
  }

  // If no input is provided, default to auto-discovery.
  if (!args.inFile && !args.url) {
    args.auto = true;
  }

  const providedInputs = [args.inFile ? 1 : 0, args.url ? 1 : 0, args.auto ? 1 : 0].reduce((a, b) => a + b, 0);
  if (providedInputs !== 1) {
    throw new Error('Provide exactly one input mode: --in, --url, or --auto.');
  }

  if (!args.outFile) {
    throw new Error('Missing required --out <output.html>.');
  }

  const outPath = path.resolve(process.cwd(), args.outFile);

  let html;
  let inferredBaseHref = '';
  let resolvedElementIdFromIndex = '';

  if (args.auto) {
    const indexUrl = (args.indexUrl || 'https://tc39.es/ecma262/multipage/').trim();
    if (!indexUrl) {
      throw new Error('Missing --index-url value.');
    }

    const indexHtml = await fetchText(indexUrl);
    const link = resolveSectionLinkFromMultipageIndexHtml(indexHtml, args.section.trim());
    if (!link) {
      throw new Error(`Could not find section '${args.section}' in multipage index: ${indexUrl}`);
    }

    const resolvedUrl = new URL(link.filePart || link.href, indexUrl).toString();
    resolvedElementIdFromIndex = link.fragment || '';

    html = await fetchText(resolvedUrl);
    inferredBaseHref = resolvedUrl;
  } else if (args.url) {
    const urlString = args.url.trim();
    html = await fetchText(urlString);
    inferredBaseHref = urlString;
  } else {
    const inputPath = path.resolve(process.cwd(), args.inFile);
    if (!fs.existsSync(inputPath)) {
      throw new Error(`Input file not found: ${inputPath}`);
    }
    html = fs.readFileSync(inputPath, 'utf8');
  }

  let elementId = (args.id || '').trim();
  if (!elementId && resolvedElementIdFromIndex) {
    elementId = resolvedElementIdFromIndex;
  }

  if (!elementId) {
    elementId = resolveSectionIdFromHtml(html, args.section.trim());
  }

  if (!elementId) {
    throw new Error(
      `Could not infer an element id for section '${args.section}'. Try passing --id sec-... explicitly.`
    );
  }

  const extracted = extractElementById(html, elementId);

  let outText = extracted.html;
  if (args.wrap) {
    const baseHref = args.baseHref || inferredBaseHref || '';
    outText = wrapAsStandaloneHtml(outText, {
      title: `ECMA-262 ${args.section}`,
      baseHref,
    });
  }

  writeTextPreserveEol(outPath, outText);

  // eslint-disable-next-line no-console
  console.log(`Extracted section ${args.section} (id=${elementId}, tag=${extracted.tagName}) -> ${outPath}`);
}

if (require.main === module) {
  main().catch((err) => {
    // eslint-disable-next-line no-console
    console.error(err && err.message ? err.message : err);
    process.exitCode = 1;
  });
}
