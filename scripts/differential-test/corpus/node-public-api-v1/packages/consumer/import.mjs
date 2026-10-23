import fs from 'node:fs';
import selected from 'conditional';
import feature from 'conditional/feature';
const actual = [selected, feature];
if (JSON.stringify(actual) !== JSON.stringify(['import-selected', 'public-feature'])) throw new Error('Import condition mismatch.');
fs.writeFileSync('result.json', JSON.stringify(actual));
