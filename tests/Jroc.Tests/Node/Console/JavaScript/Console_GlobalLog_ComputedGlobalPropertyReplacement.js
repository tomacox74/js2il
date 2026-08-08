const originalConsole = console;
globalThis["console"] = {
    log(value) {
        originalConsole.log("computed", value);
    }
};

console.log("global");
