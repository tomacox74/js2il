"use strict";

function formatSection(section) {
    return "section:" + section;
}

async function main() {
    const label = formatSection("27.3");
    await Promise.resolve(null);
    console.log(label);
}

main().catch((err) => {
    console.error(err && err.message ? err.message : err);
    process.exitCode = 1;
});
