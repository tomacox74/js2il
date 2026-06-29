/**
 * Thin wrapper around two npm-run-all2 utilities:
 *
 *  taskHeader(nameAndArgs)
 *    Formats the "> task-name args" header that npm-run-all2 prints
 *    before each task executes (from npm-run-all2/lib/create-header).
 *
 *  filterTasks(taskListCsv, patternsCsv)
 *    Returns every task name from taskListCsv that is matched by at
 *    least one pattern in patternsCsv.  Patterns follow npm-run-all2's
 *    glob rules: ":" and "/" are interchangeable separators, so
 *    "test:*" matches "test:unit", "test:integration", etc.
 *    C#-friendly – accepts and returns comma-separated strings.
 *
 * Note: this wrapper uses createHeader directly from npm-run-all2 and
 * provides its own glob-to-regex conversion for filterTasks instead of
 * pulling in picomatch (which generates regex lookahead assertions not
 * yet supported in JROC's IL back-end).
 */
'use strict';

const createHeader = require('npm-run-all2/lib/create-header');

/**
 * Returns the npm-run-all2 run-header for a task, e.g. "> build".
 *
 * @param {string} nameAndArgs - Task name (and optional arguments).
 * @returns {string} Trimmed header string.
 */
function taskHeader(nameAndArgs) {
  return createHeader(nameAndArgs, null, false, null).trim();
}

/**
 * Converts a glob pattern into a RegExp using npm-run-all2's convention:
 * ":" and "/" are treated as equivalent path separators, and "*" matches
 * any sequence of non-separator characters.
 *
 * @param {string} pattern - A glob such as "test:*" or "build".
 * @returns {RegExp}
 */
function patternToRegex(pattern) {
  const norm = pattern.replace(/[:/]/g, '/');
  const escaped = norm.replace(/[.+^${}()|[\]\\]/g, '\\$&').replace(/\*/g, '[^/]*');
  return new RegExp('^' + escaped + '$');
}

/**
 * Filter npm-script names using glob patterns.
 *
 * @param {string} taskListCsv - Comma-separated list of available task names.
 * @param {string} patternsCsv - Comma-separated list of glob patterns.
 * @returns {string} Comma-separated matched task names (empty string if none).
 */
function filterTasks(taskListCsv, patternsCsv) {
  const tasks = taskListCsv.split(',');
  const patterns = patternsCsv.split(',');
  const seen = new Set();
  const result = [];

  for (const pat of patterns) {
    const re = patternToRegex(pat.trim());
    for (const task of tasks) {
      if (re.test(task.replace(/[:/]/g, '/')) && !seen.has(task)) {
        seen.add(task);
        result.push(task);
      }
    }
  }

  return result.join(',');
}

module.exports = { taskHeader, filterTasks };
