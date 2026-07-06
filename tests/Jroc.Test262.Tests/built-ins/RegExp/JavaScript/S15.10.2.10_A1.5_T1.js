var arr = /\r/.exec("\u000D");
if ((arr === null) || (arr[0] !== "\u000D")) {
  throw new Test262Error('#1: var arr = /\\r/.exec("\\u000D"); arr[0] === "\\u000D". Actual. ' + (arr && arr[0]));
}

var arr = /\r\r/.exec("a\u000D\u000Db");
if ((arr === null) || (arr[0] !== "\u000D\u000D")) {
  throw new Test262Error('#2: var arr = /\\r\\r/.exec("a\\u000D\\u000Db"); arr[0] === "\\u000D\\u000D". Actual. ' + (arr && arr[0]));
}
