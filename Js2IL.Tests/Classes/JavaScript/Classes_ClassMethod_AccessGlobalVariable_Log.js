"use strict";\r\n\r\nconst GLOBAL_VALUE = "Hello from global";

class MyClass {
	logGlobal() {
		console.log(GLOBAL_VALUE);
	}
}

const instance = new MyClass();
instance.logGlobal();
