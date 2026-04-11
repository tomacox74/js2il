'use strict';

async function run() {
  var harness = require('./Compile_Scripts_ExtractEcma262SectionHtml_TestHarness');
  await harness.runHarnessCli();
}

run().catch((err) => {
  console.error(err && err.stack ? err.stack : (err && err.message ? err.message : err));
  process.exitCode = 1;
});
