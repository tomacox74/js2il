function testFunction() {
	const FUNCTION_VALUE = "Hello from function";
	
	class MyClass {
		constructor(paramValue) {
			console.log(FUNCTION_VALUE);
			console.log(paramValue);
		}
	}
	
	const instance = new MyClass("Hello from parameter");
}

testFunction();
