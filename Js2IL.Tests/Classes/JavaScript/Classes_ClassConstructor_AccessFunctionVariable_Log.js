"use strict";\r\n\r\nfunction testFunction() {
	const FUNCTION_VALUE = "Hello from function";
	
	class MyClass {
		constructor() {
			console.log(FUNCTION_VALUE);
		}
	}
	
	const instance = new MyClass();
}

testFunction();
