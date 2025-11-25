const GLOBAL_VALUE = "Hello from global";

class MyClass {
	constructor() {
		console.log(GLOBAL_VALUE);
	}
}

const instance = new MyClass();
