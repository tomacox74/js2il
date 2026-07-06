var arr = /\f/.exec("\u000C");
if ((arr === null) || (arr[0] !== "\u000C")) {
  throw new Test262Error('#1: var arr = /\\f/.exec("\\u000C"); arr[0] === "\\u000C". Actual. ' + (arr && arr[0]));
}

var arr = /\f\f/.exec("a\u000C\u000Cb");
if ((arr === null) || (arr[0] !== "\u000C\u000C")) {
  throw new Test262Error('#2: var arr = /\\f\\f/.exec("a\\u000C\\u000Cb"); arr[0] === "\\u000C\\u000C". Actual. ' + (arr && arr[0]));
}
