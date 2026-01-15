function logCase(label, before, result, after) {
	console.log(label, before, result, after);
}

// uncaptured global variable (typed as double)
let g = 3;
let before = g;
let result = --g;
let after = g;
logCase('global-double', before, result, after);

// captured variable in a function (typed as double)
let capturedGlobalDouble = 3;
function capturedDouble() {
	let before = capturedGlobalDouble;
	let result = --capturedGlobalDouble;
	let after = capturedGlobalDouble;
	logCase('captured-double', before, result, after);
}
capturedDouble();

// argument passed in as double
function argDouble(n) {
	let before = n;
	let result = --n;
	let after = n;
	logCase('arg-double', before, result, after);
}
argDouble(3);

// uncaptured global variable (typed as string)
let gs = '3';
before = gs;
result = --gs;
after = gs;
logCase('global-string', before, result, after);

// captured variable in a function (typed as string)
let capturedGlobalString = '3';
function capturedString() {
	let before = capturedGlobalString;
	let result = --capturedGlobalString;
	let after = capturedGlobalString;
	logCase('captured-string', before, result, after);
}
capturedString();

// argument passed in as string
function argString(n) {
	let before = n;
	let result = --n;
	let after = n;
	logCase('arg-string', before, result, after);
}
argString('3');
