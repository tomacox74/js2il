var dateStr = (new Date(0)).toISOString();

assert.sameValue(dateStr[dateStr.length - 1], "Z", 'dateStr[dateStr.length - 1]');
