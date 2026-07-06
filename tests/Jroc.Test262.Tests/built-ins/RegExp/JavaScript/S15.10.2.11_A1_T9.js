var arr = /((((((((((A))))))))))\10\9\8\7\6\5\4\3\2\1/.exec("AAAAAAAAAAA");

if ((arr === null) || (arr[0] !== "AAAAAAAAAAA")) {
  throw new Test262Error('#1: var arr = /((((((((((A))))))))))\\10\\9\\8\\7\\6\\5\\4\\3\\2\\1/.exec("AAAAAAAAAAA"); arr[0] === "AAAAAAAAAAA". Actual. ' + (arr && arr[0]));
}

for (var i = 1; i <= 10; i++) {
    if ((arr === null) || (arr[i] !== "A")) {
    throw new Test262Error('#2: var arr = /((((((((((A))))))))))\\10\\9\\8\\7\\6\\5\\4\\3\\2\\1/.exec("AAAAAAAAAAA"); arr[' + i + '] === "A". Actual. ' + (arr && arr[i]));
  }
}
