"use strict";

const assert = require("node:assert/strict");
const test = require("node:test");
const {
  findPreviousCommonPatchVersion,
  parseStableVersion,
  parseVersionCore,
} = require("./findPreviousJrocPackageVersion");

test("parses only stable three-part versions", () => {
  assert.deepEqual(parseStableVersion("v0.12.14"), {
    major: 0,
    minor: 12,
    patch: 14,
  });
  assert.equal(parseStableVersion("0.12.14-preview.1"), null);
  assert.equal(parseStableVersion("0.12"), null);
});

test("uses a prerelease current version's numeric release line", () => {
  assert.deepEqual(parseVersionCore("0.12.14-preview.1"), {
    major: 0,
    minor: 12,
    patch: 14,
  });
  assert.equal(findPreviousCommonPatchVersion("0.12.14-preview.1", [
    ["0.12.12", "0.12.13", "0.12.14-preview.1"],
    ["0.12.12", "0.12.13", "0.12.14-preview.1"],
  ]), "0.12.13");
});

test("finds the highest prior patch shared by every package", () => {
  const previous = findPreviousCommonPatchVersion("0.12.14", [
    ["0.12.11", "0.12.12", "0.12.13", "0.12.14"],
    ["0.12.11", "0.12.13", "0.12.14"],
  ]);

  assert.equal(previous, "0.12.13");
});

test("ignores prereleases and versions from other release lines", () => {
  const previous = findPreviousCommonPatchVersion("0.12.14", [
    ["0.11.99", "0.12.13-preview.1", "0.12.12", "0.13.0"],
    ["0.11.99", "0.12.13-preview.1", "0.12.12", "0.13.0"],
  ]);

  assert.equal(previous, "0.12.12");
});

test("returns null when the current release line has no prior patch", () => {
  assert.equal(findPreviousCommonPatchVersion("0.12.0", [
    ["0.11.9", "0.12.0"],
    ["0.11.9", "0.12.0"],
  ]), null);
});
