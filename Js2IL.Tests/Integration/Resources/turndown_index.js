'use strict';

// Minimal stub of the `turndown` npm package for compiler-only tests.
// Enough surface area for scripts/ECMA262/convertEcmaExtractHtmlToMarkdown.js to compile.

function TurndownService() {}

TurndownService.prototype.addRule = function addRule(_name, _rule) {
  // no-op
};

TurndownService.prototype.turndown = function turndown(html) {
  return String(html || '');
};

module.exports = TurndownService;
