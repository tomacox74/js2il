const path = require('path');
// Print only basename and name-without-ext of a synthetic path to avoid host file variability
const p = path.join(__dirname, 'probe.name.dll');
console.log(path.basename(p));
// The test harness runs in a per-run temp directory, so the final segment of __dirname is a GUID.
// Print the parent directory name (e.g. "Node.ExecutionTests") for stability.
const d = path.dirname(p);
console.log(path.basename(path.dirname(d)));
