function breakCase(abruptFromInnerFinally) {
  let log = "";

  outer: for (;;) {
    try {
      try {
        break outer;
      } finally {
        log += "inner";
        if (abruptFromInnerFinally) {
          break outer;
        }
      }
    } finally {
      log += "outer";
      if (false) {
        break outer;
      }
    }
  }

  return log;
}

function continueCase(abruptFromInnerFinally) {
  let log = "";

  outer: for (let index = 0; index < 1; index++) {
    try {
      try {
        continue outer;
      } finally {
        log += "inner";
        if (abruptFromInnerFinally) {
          continue outer;
        }
      }
    } finally {
      log += "outer";
      if (false) {
        continue outer;
      }
    }
  }

  return log;
}

console.log(breakCase(false));
console.log(breakCase(true));
console.log(continueCase(false));
console.log(continueCase(true));
