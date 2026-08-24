#!/usr/bin/env node
"use strict";

const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const {
  getFailedTests,
  parseFailedTestsFromLog,
  runGh,
  truncateCommandFailureOutput,
} = require("./summarizePrCiFailures");

test("runGh accepts GitHub job logs larger than Node's default buffer", async () => {
  const tempDirectory = await fs.mkdtemp(path.join(os.tmpdir(), "jroc-gh-log-test-"));
  const fakeGh = path.join(tempDirectory, "gh");
  const originalPath = process.env.PATH;

  try {
    await fs.writeFile(
      fakeGh,
      "#!/usr/bin/env node\nprocess.stdout.write(\"x\".repeat(2 * 1024 * 1024));\n",
      { mode: 0o755 });
    process.env.PATH = `${tempDirectory}${path.delimiter}${originalPath ?? ""}`;

    assert.equal(runGh(["run", "view"]).length, 2 * 1024 * 1024);
  } finally {
    if (originalPath === undefined) {
      delete process.env.PATH;
    } else {
      process.env.PATH = originalPath;
    }
    await fs.rm(tempDirectory, { recursive: true, force: true });
  }
});

test("parses failed test names and assertion details from Actions logs", () => {
  const failures = parseFailedTestsFromLog([
    "build\tTest\t2026-08-18T16:55:52.4648660Z [xUnit.net 00:00:59.11]     Jroc.Tests.Array.DenseWrites [FAIL]",
    "build\tTest\t2026-08-18T16:55:52.5267024Z   Failed Jroc.Tests.Array.DenseWrites [21 ms]",
    "build\tTest\t2026-08-18T16:55:52.5394165Z   Error Message:",
    "build\tTest\t2026-08-18T16:55:52.5415082Z    Assert.InRange() Failure: Value not in range",
    "build\tTest\t2026-08-18T16:55:52.5434125Z Range:  (0 - 131072)",
    "build\tTest\t2026-08-18T16:55:52.5463809Z Actual: 144920",
    "build\tTest\t2026-08-18T16:55:52.5473809Z   Stack Trace:",
  ].join("\n"));

  assert.deepEqual(failures, [{
    name: "Jroc.Tests.Array.DenseWrites",
    details: [
      "Assert.InRange() Failure: Value not in range",
      "Range:  (0 - 131072)",
      "Actual: 144920",
    ],
  }]);
});

test("falls back to the Actions job-log API when gh run view returns no log", () => {
  const calls = [];
  const failures = getFailedTests({
    name: "build",
    detailsUrl: "https://github.com/octo/example/actions/runs/123/job/456",
  }, "octo/example", (args) => {
    calls.push(args);
    if (args[0] === "run") {
      return "";
    }

    return [
      "2026-08-24T16:36:15.9582466Z [xUnit.net 00:00:21.20]     Example.Tests.Failure [FAIL]",
      "2026-08-24T16:36:15.9707835Z   Failed Example.Tests.Failure [2 s]",
      "2026-08-24T16:36:15.9708764Z   Error Message:",
      "2026-08-24T16:36:15.9709680Z    Expected: true",
      "2026-08-24T16:36:15.9711107Z    Actual: false",
    ].join("\n");
  });

  assert.deepEqual(calls, [
    ["run", "view", "123", "--job", "456", "--log-failed", "--repo", "octo/example"],
    ["api", "repos/octo/example/actions/jobs/456/logs"],
  ]);
  assert.deepEqual(failures, [{
    name: "Example.Tests.Failure",
    details: [
      "Expected: true",
      "Actual: false",
    ],
  }]);
});

test("truncates oversized command errors from the tail", () => {
  const output = `prefix-${"x".repeat(10 * 1024)}-useful-tail`;
  const truncated = truncateCommandFailureOutput(output);

  assert.match(truncated, /^\[\.\.\. \d+ characters omitted \.\.\.\]/);
  assert.match(truncated, /useful-tail$/);
  assert.ok(truncated.length < output.length);
});
