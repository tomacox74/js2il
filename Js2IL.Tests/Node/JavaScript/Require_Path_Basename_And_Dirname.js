const path = require('path');
// Print only basename and name-without-ext of a synthetic path to avoid host file variability
const p = path.join(__dirname, 'probe.name.dll');
console.log(path.basename(p));
// Print only the last segment of dirname for stability
console.log(path.basename(path.dirname(p)));
