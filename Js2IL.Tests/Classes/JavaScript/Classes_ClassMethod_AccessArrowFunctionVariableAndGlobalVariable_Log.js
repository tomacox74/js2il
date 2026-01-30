"use strict";

const GLOBAL_VALUE = "Hello from global";

const testArrowFunction = () => {
	const FUNCTION_VALUE = "Hello from function";
	
	class MyClass {
		logValues() {
			console.log(GLOBAL_VALUE);
			console.log(FUNCTION_VALUE);
		}
	}
	
	const instance = new MyClass();
	instance.logValues();
};

testArrowFunction();
