"use strict";

import {
    importedModuleFilenameMatches,
    importedModulePathMatches,
    importedUrl,
    importedUrlHasFileScheme,
} from "./Import_ImportMeta_Url_Lib.mjs";

const url = require("node:url");
const importMetaPath = url.fileURLToPath(import.meta.url).split("\\").join("/");
const assetPath = url.fileURLToPath(new url.URL("./fixtures/main.txt", import.meta.url)).split("\\").join("/");
const importedPath = url.fileURLToPath(importedUrl).split("\\").join("/");
const importedAssetPath = url.fileURLToPath(new url.URL("./fixtures/lib.txt", importedUrl)).split("\\").join("/");

console.log("main has file url:", import.meta.url.startsWith("file://"));
console.log("main path matches filename:", importMetaPath === __filename.split("\\").join("/"));
console.log("main asset path:", assetPath.endsWith("/fixtures/main.txt"));
console.log("lib has file url:", importedUrlHasFileScheme);
console.log("lib path:", importedPath.endsWith("/Import_ImportMeta_Url_Lib"));
console.log("lib asset path:", importedAssetPath.endsWith("/fixtures/lib.txt"));
console.log("lib module.filename:", importedModuleFilenameMatches);
console.log("lib module.path:", importedModulePathMatches);
