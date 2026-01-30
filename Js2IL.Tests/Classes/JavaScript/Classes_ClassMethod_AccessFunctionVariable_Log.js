"use strict";

function testFunction() {
	const FUNCTION_VALUE = "Hello from function";
	
	class MyClass {
		logValue() {
			console.log(FUNCTION_VALUE);
		}
	}
	
	const instance = new MyClass();
	instance.logValue();
}

testFunction();
