var arr = /\v/.exec("\u000B");
if ((arr === null) || (arr[0] !== "\u000B")) {
  throw new Test262Error('#1: var arr = /\\v/.exec("\\u000B"); arr[0] === "\\u000B". Actual. ' + (arr && arr[0]));
}

var arr = /\v\v/.exec("a\u000B\u000Bb");
if ((arr === null) || (arr[0] !== "\u000B\u000B")) {
  throw new Test262Error('#2: var arr = /\\v\\v/.exec("a\\u000B\\u000Bb"); arr[0] === "\\u000B\\u000B". Actual. ' + (arr && arr[0]));
}
