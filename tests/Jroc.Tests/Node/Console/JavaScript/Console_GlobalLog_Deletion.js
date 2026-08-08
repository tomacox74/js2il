const originalConsole = console;
delete globalThis.console;

try {
    console.log("unreachable");
} catch (error) {
    originalConsole.log("deleted", error instanceof ReferenceError);
}
