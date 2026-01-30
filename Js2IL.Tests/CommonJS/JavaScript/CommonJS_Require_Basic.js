"use strict";\r\n\r\n[1].map((x) => x);

require('./CommonJS_Require_Dependency');
console.log('CommonJS_Require_Basic has been loaded');

const moduleName = "CommonJS_Require_Basic";

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
