'use strict';

const GENERATED_TIMESTAMP_PREFIX = '> Last generated (UTC): ';

function formatGeneratedTimestamp(date = new Date()) {
  if (!(date instanceof Date) || Number.isNaN(date.getTime())) {
    throw new Error('Expected a valid Date for generated markdown metadata.');
  }

  return date.toISOString().replace(/\.\d{3}Z$/, 'Z');
}

function buildGeneratedTimestampLine(date = new Date()) {
  return `${GENERATED_TIMESTAMP_PREFIX}${formatGeneratedTimestamp(date)}`;
}

function isGeneratedTimestampLine(line) {
  return typeof line === 'string' && line.trim().startsWith(GENERATED_TIMESTAMP_PREFIX);
}

module.exports = {
  buildGeneratedTimestampLine,
  isGeneratedTimestampLine,
};
