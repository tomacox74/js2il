[1].map((x) => x);

const moduleName = "CommonJS_Require_NestedNameConflict/helpers/b";

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
