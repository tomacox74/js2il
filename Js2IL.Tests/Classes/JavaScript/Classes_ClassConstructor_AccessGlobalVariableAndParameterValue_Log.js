"use strict";

const GLOBAL_VALUE = "Hello from global";

class MyClass {
	constructor(paramValue) {
		console.log(GLOBAL_VALUE);
		console.log(paramValue);
	}
}

const instance = new MyClass("Hello from parameter");
