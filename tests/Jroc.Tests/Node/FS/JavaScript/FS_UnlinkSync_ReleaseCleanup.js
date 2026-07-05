"use strict";

const fs = require("fs");
const path = require("path");
const os = require("os");

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const root = path.join(os.tmpdir(), `jroc-release-cleanup-${uniqueSuffix}`);
const prBody = path.join(root, "pr-body.md");
const releaseNotes = path.join(root, "release-notes.md");

fs.rmSync(root, { recursive: true, force: true });
fs.mkdirSync(root, { recursive: true });

try {
    fs.writeFileSync(prBody, "Patch release body", "utf8");
    fs.writeFileSync(releaseNotes, "# Release notes", "utf8");

    console.log("Before:", fs.existsSync(prBody), fs.existsSync(releaseNotes));

    fs.unlinkSync(prBody);
    fs.unlinkSync(releaseNotes);

    console.log("After:", fs.existsSync(prBody), fs.existsSync(releaseNotes));

    try {
        fs.unlinkSync(prBody);
    } catch (e) {
        console.log("MissingError:", e.message.includes("ENOENT:"), e.message.includes("pr-body.md"));
    }
} finally {
    fs.rmSync(root, { recursive: true, force: true });
}
