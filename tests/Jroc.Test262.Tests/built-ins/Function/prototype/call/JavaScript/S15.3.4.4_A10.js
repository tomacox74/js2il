// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: The Function.prototype.call.length property has the attribute ReadOnly
es5id: 15.3.4.4_A10
description: >
    Checking if varying the Function.prototype.call.length property
    fails
includes: [propertyHelper.js]
---*/

function isWritable(obj, name, verifyProp, value) {
  var newValue = arguments.length > 3 ? value : "unlikelyValue";
  var verifiedName = verifyProp || name;
  var oldValue = obj[verifiedName];
  var writeSucceeded = false;

  try {
    obj[name] = newValue;
    writeSucceeded = Object.is(obj[verifiedName], newValue);
  } catch (error) {
    writeSucceeded = false;
  }

  try {
    obj[name] = oldValue;
  } catch (error) {
  }

  return writeSucceeded;
}

assert(
  Function.prototype.call.hasOwnProperty('length'),
  'Function.prototype.call.hasOwnProperty(\'length\') must return true'
);

var obj = Function.prototype.call.length;

verifyNotWritable(Function.prototype.call, "length", null, function() {
  return "shifted";
});

assert.sameValue(
  Function.prototype.call.length,
  obj,
  'The value of Function.prototype.call.length is expected to equal the value of obj'
);

// TODO: Convert to verifyProperty() format.
