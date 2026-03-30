"use strict";

export const importedUrl = import.meta.url;
export const importedUrlHasFileScheme = importedUrl.startsWith("file://");
export const importedModuleFilenameMatches = module.filename === __filename;
export const importedModulePathMatches = module.path.split("\\").join("/") === __dirname.split("\\").join("/");
