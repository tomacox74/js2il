// Test that non-literal import() is rejected during compilation
// This should fail validation

const moduleName = "./some-module";
import(moduleName); // Should cause validation error
