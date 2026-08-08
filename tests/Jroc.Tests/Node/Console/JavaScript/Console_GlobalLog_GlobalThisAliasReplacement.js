const globals = globalThis;
const originalConsole = console;
globals.console = {
    log(value) {
        originalConsole.log("aliased", value);
    }
};

console.log("global");
