"use strict";\r\n\r\nconst GLOBAL_VALUE = "Hello from global";

function testFunction() {
	const FUNCTION_VALUE = "Hello from function";
	
	class MyClass {
		constructor(paramValue) {
			console.log(GLOBAL_VALUE);
			console.log(FUNCTION_VALUE);
			console.log(paramValue);
		}
	}
	
	const instance = new MyClass("Hello from parameter");
}

testFunction();
