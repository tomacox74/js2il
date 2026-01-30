"use strict";\r\n\r\nconst GLOBAL_VALUE = "Hello from global";

function testFunction() {
	const FUNCTION_VALUE = "Hello from function";
	
	class MyClass {
		constructor() {
			console.log(GLOBAL_VALUE);
			console.log(FUNCTION_VALUE);
		}
	}
	
	const instance = new MyClass();
}

testFunction();
