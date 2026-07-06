var date = new Date(-0);

assert.sameValue(date.getTime(), +0, "TimeClip does not return negative zero");
