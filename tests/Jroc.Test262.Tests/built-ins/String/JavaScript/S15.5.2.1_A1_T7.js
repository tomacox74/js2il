var __stored__Object__prototype__toString = Object.prototype.toString;

Object.prototype.toString = function() {
  return "SHIFTED"
};

var __str = new String({});

Object.prototype.toString = __stored__Object__prototype__toString;

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (typeof __str !== "object") {
  throw new Test262Error('#1: __str = new String({}); typeof __str === "object". Actual: typeof __str ===' + typeof __str);
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#1.5
if (__str.constructor !== String) {
  throw new Test262Error('#1.5: __str = new String({}); __str.constructor === String. Actual: __str.constructor ===' + __str.constructor);
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (__str != "SHIFTED") {
  throw new Test262Error('#2: Object.prototype.toString=function(){return "SHIFTED"}; __str = new String({}); __str =="SHIFTED". Actual: __str ==' + __str);
}
//
//////////////////////////////////////////////////////////////////////////////
