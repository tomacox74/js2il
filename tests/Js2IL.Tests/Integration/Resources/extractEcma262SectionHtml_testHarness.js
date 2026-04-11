'use strict';

const fs = require('fs');
const http = require('node:http');
const https = require('node:https');
const path = require('path');
const { URL: NodeUrl } = require('node:url');

function normalize(text, outFile) {
  if (text.includes(' -> ')) {
    text = text.substring(0, text.indexOf(' -> ') + 4) + '<outFile>';
  }

  return text
    .replace(new RegExp(outFile.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), '<outFile>')
    .replace(/127\.0\.0\.1:\d+/g, '127.0.0.1:<port>')
    .replace(/\r\n/g, '\n');
}

function parseArgs(argv) {
  const args = {
    section: '',
    url: '',
    auto: false,
    indexUrl: '',
    outFile: '',
    id: '',
  };

  for (let i = 2; i < argv.length; i++) {
    const value = argv[i];

    if (value === '--section') {
      args.section = argv[i + 1] || '';
      i++;
      continue;
    }

    if (value === '--url') {
      args.url = argv[i + 1] || '';
      i++;
      continue;
    }

    if (value === '--auto') {
      args.auto = true;
      continue;
    }

    if (value === '--index-url') {
      args.indexUrl = argv[i + 1] || '';
      i++;
      continue;
    }

    if (value === '--out') {
      args.outFile = argv[i + 1] || '';
      i++;
      continue;
    }

    if (value === '--id') {
      args.id = argv[i + 1] || '';
      i++;
    }
  }

  return args;
}

function fetchText(urlString, maxRedirects = 5) {
  return new Promise((resolve, reject) => {
    let urlObj;
    try {
      urlObj = new NodeUrl(urlString);
    } catch {
      reject(new Error(`Invalid URL: ${urlString}`));
      return;
    }

    const transport = urlObj.protocol === 'https:' ? https : http;
    const req = transport.request(
      urlObj,
      {
        method: 'GET',
        headers: {
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

          const nextUrl = new NodeUrl(location, urlObj).toString();
          res.resume();
          fetchText(nextUrl, maxRedirects - 1).then(resolve, reject);
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
        res.on('end', () => {
          resolve(data);
        });
      }
    );

    req.on('error', reject);
    req.end();
  });
}

function escapeRegExp(text) {
  return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function findTagNameAt(html, tagStart) {
  if (tagStart < 0 || html[tagStart] !== '<') {
    return '';
  }

  let index = tagStart + 1;
  while (index < html.length) {
    const ch = html[index];
    if (ch === ' ' || ch === '\t' || ch === '\r' || ch === '\n' || ch === '>' || ch === '/') {
      break;
    }
    index++;
  }

  return html.substring(tagStart + 1, index);
}

function extractElementById(html, elementId) {
  const idDouble = `id="${elementId}"`;
  const idSingle = `id='${elementId}'`;

  let idIndex = html.indexOf(idDouble);
  if (idIndex < 0) {
    idIndex = html.indexOf(idSingle);
  }

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

  const tagRe = new RegExp(`<\\/?${escapeRegExp(tagName)}\\b[^>]*>`, 'gi');
  tagRe.lastIndex = tagStart;

  let depth = 0;
  let startIndex = -1;

  while (true) {
    const match = tagRe.exec(html);
    if (!match) {
      break;
    }

    const token = match[0];
    if (startIndex < 0) {
      startIndex = match.index;
    }

    if (token.startsWith('</')) {
      depth--;
      if (depth === 0) {
        return {
          tagName,
          html: html.substring(startIndex, tagRe.lastIndex),
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
  const re = new RegExp(
    `<a\\b[^>]*href=(?:\"([^\"]+)\"|'([^']+)')[^>]*>(?:(?!<\\/a>)[\\s\\S]){0,4000}?<span\\b[^>]*class=(?:\"secnum\"|'secnum')[^>]*>\\s*${sectionEsc}\\s*<\\/span>(?:(?!<\\/a>)[\\s\\S]){0,4000}?<\\/a>`,
    'i'
  );
  const match = re.exec(indexHtml);
  const href = (match && (match[1] || match[2])) || '';
  if (!href) {
    return null;
  }

  const hashIndex = href.indexOf('#');
  return {
    href,
    filePart: hashIndex >= 0 ? href.substring(0, hashIndex) : href,
    fragment: hashIndex >= 0 ? href.substring(hashIndex + 1) : '',
  };
}

function wrapAsStandaloneHtml(extractedHtml, title, baseHref) {
  const baseTag = baseHref ? `  <base href="${baseHref}">\n` : '';
  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
${baseTag}  <style>
    body { font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; margin: 2rem; line-height: 1.35; }
    emu-clause, emu-intro, emu-annex, emu-section, emu-subsection, emu-table, emu-figure, emu-note, emu-example, emu-alg, emu-grammar, emu-prodref, emu-xref { display: block; }
    emu-note { border-left: 4px solid #ccc; padding-left: 1rem; margin-left: 0; }
    table { border-collapse: collapse; }
    th, td { border: 1px solid #ddd; padding: 0.25rem 0.5rem; vertical-align: top; }
    pre { background: #f6f8fa; padding: 0.75rem; overflow: auto; }
    code { background: #f6f8fa; padding: 0 0.25rem; border-radius: 3px; }
  </style>
  <title>${title}</title>
</head>
<body>
${extractedHtml}
</body>
</html>
`;
}

async function runHarnessCli() {
  const args = parseArgs(process.argv);
  if (!args.section) {
    throw new Error('Missing required --section (e.g. 27.3).');
  }

  if (!args.outFile) {
    throw new Error('Missing required --out <output.html>.');
  }

  const providedInputs = (args.url ? 1 : 0) + (args.auto ? 1 : 0);
  if (providedInputs !== 1) {
    throw new Error('Provide exactly one input mode: --url or --auto.');
  }

  let html;
  let elementId = (args.id || '').trim();
  let baseHref = '';

  if (args.auto) {
    const indexUrl = args.indexUrl.trim();
    const indexHtml = await fetchText(indexUrl);
    const link = resolveSectionLinkFromMultipageIndexHtml(indexHtml, args.section.trim());
    if (!link) {
      throw new Error(`Could not find section '${args.section}' in multipage index: ${indexUrl}`);
    }

    const resolvedUrl = new NodeUrl(link.filePart || link.href, indexUrl).toString();
    html = await fetchText(resolvedUrl);
    baseHref = resolvedUrl;
    if (!elementId) {
      elementId = link.fragment || '';
    }
  } else {
    const urlString = args.url.trim();
    html = await fetchText(urlString);
    baseHref = urlString;
  }

  if (!elementId) {
    throw new Error(`Could not infer an element id for section '${args.section}'. Try passing --id sec-... explicitly.`);
  }

  const extracted = extractElementById(html, elementId);
  const outPath = path.join(process.cwd(), args.outFile);
  const outText = wrapAsStandaloneHtml(extracted.html, `ECMA-262 ${args.section}`, baseHref);
  fs.writeFileSync(outPath, outText, 'utf8');

  console.log(normalize(`Extracted section ${args.section} (id=${elementId}, tag=${extracted.tagName}) -> ${outPath}`, outPath));
  console.log(normalize(fs.readFileSync(outPath, 'utf8'), outPath));
}

exports.runHarnessCli = runHarnessCli;
