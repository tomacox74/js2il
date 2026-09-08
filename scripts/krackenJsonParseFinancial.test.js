"use strict";

const assert = require("node:assert/strict");
const crypto = require("node:crypto");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const scenarioDirectory = path.join(
  __dirname,
  "../tests/performance/Benchmarks/Scenarios/kracken-1.1"
);

test("financial JSON workload defers exactly 1000 full parses until invocation", () => {
  let workload;
  let iterations;
  let parseCalls = 0;
  let registrations = 0;
  const context = vm.createContext({
    JSON: {
      parse(text) {
        parseCalls++;
        assert.equal(text, context.data);
        return JSON.parse(text);
      },
    },
    runTest(callback, count) {
      registrations++;
      workload = callback;
      iterations = count;
    },
  });

  for (const name of ["json-parse-financial-data.js", "json-parse-financial.js"]) {
    vm.runInContext(fs.readFileSync(path.join(scenarioDirectory, name), "utf8"), context, {
      filename: name,
    });
  }

  assert.equal(registrations, 1);
  assert.equal(typeof workload, "function");
  assert.equal(iterations, 1000);
  assert.equal(parseCalls, 0);
  assert.equal(context.x, undefined);
  assert.equal(Buffer.byteLength(context.data, "utf8"), 5534);
  assert.equal(
    crypto.createHash("sha256").update(context.data).digest("hex"),
    "94998a8cd5712e05b23d4745037119085dc0cc60d0cb2460715ae7fffba6b5bf"
  );

  const expected = JSON.parse(context.data);
  assert.equal(expected.holdings.length, 18);
  assert.equal(expected.pendingOrders.length, 2);
  assert.equal(expected.holdings[0].stock, "GOOG");
  assert.equal(expected.id, 219948);

  let previous;
  for (let invocation = 1; invocation <= 2; invocation++) {
    for (let i = 0; i < iterations; i++) {
      workload();
    }
    assert.equal(parseCalls, invocation * 1000);
    assert.deepEqual(context.x, expected);
    assert.notEqual(context.x, previous);
    previous = context.x;
  }
});
