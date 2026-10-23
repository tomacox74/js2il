import fs from 'node:fs';
import { value } from './modules/typed/value.js';
import extra from './modules/value.mjs';
const actual = [value, extra];
if (JSON.stringify(actual) !== JSON.stringify(['package-type-module', 37])) throw new Error('ESM relative/type mismatch.');
fs.writeFileSync('result.json', JSON.stringify(actual));
