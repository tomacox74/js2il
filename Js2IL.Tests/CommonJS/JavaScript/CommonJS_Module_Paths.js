// Test module.paths - the search paths for the module
// This is an array of directories where require() looks for modules

console.log("module.paths type:", typeof module.paths);
console.log("module.paths is array:", Array.isArray(module.paths));

// Paths should be an array of strings
if (Array.isArray(module.paths) && module.paths.length > 0) {
    console.log("first path type:", typeof module.paths[0]);
    console.log("paths length > 0:", module.paths.length > 0);
} else {
    console.log("module.paths is empty array:", module.paths.length === 0);
}
