/**
 * Use the same class names amd functions in multiple files to ensure no name collisions occur
 */

const moduleName = "CommonJS_Require_Dependency";

class CommonClassName {
    Log() {
        console.log(`class from ${moduleName} has been loaded`);
    }
}

function commonFunctionName() {
    console.log(`Function from ${moduleName} has been called`);
}

new CommonClassName().Log();
commonFunctionName();
