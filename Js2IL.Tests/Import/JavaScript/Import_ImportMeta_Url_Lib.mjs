"use strict";

export const importedUrlHasFileScheme = import.meta.url.startsWith("file://");
export const importedFilePathEndsWith = require("node:url").fileURLToPath(import.meta.url).split("\\").join("/").endsWith("/Import_ImportMeta_Url_Lib");
export const importedAssetPathEndsWith = require("node:url").fileURLToPath(new (require("node:url").URL)("./fixtures/lib.txt", import.meta.url)).split("\\").join("/").endsWith("/fixtures/lib.txt");
