/*---
description: Resource scopes preserve completions across nested cleanup and iterator closing.
flags: [async]
includes: [asyncHelpers.js, compareArray.js]
---*/

asyncTest(async function () {
  var events = [];
  function resource(name) {
    return {
      name,
      [Symbol.asyncDispose]() { events.push(name); }
    };
  }

  async function nestedReturn() {
    await using outer = resource("outer");
    {
      await using inner = resource("inner");
      return 17;
    }
  }
  assert.sameValue(await nestedReturn(), 17);
  assert.compareArray(events, ["inner", "outer"]);

  events = [];
  outer: {
    await using value = resource("break");
    break outer;
  }
  events.push("after");
  assert.compareArray(events, ["break", "after"]);

  events = [];
  for (await using value of [resource("continue"), resource("break"), resource("unused")]) {
    if (value.name === "continue") continue;
    break;
  }
  assert.compareArray(events, ["continue", "break"]);

  events = [];
  function* values() {
    try { yield resource("inner"); }
    finally { events.push("close"); }
  }
  async function returnFromLoop() {
    await using outer = resource("outer");
    for (await using inner of values()) return 19;
  }
  assert.sameValue(await returnFromLoop(), 19);
  assert.compareArray(events, ["inner", "close", "outer"]);

  events = [];
  async function cleanupInFinally() {
    try { return 23; }
    finally {
      await using value = resource("finally");
    }
  }
  assert.sameValue(await cleanupInFinally(), 23);
  assert.compareArray(events, ["finally"]);

  events = [];
  var original = new Error();
  async function throwThroughFinally() {
    try { throw original; }
    finally {
      await using value = resource("finally");
    }
  }
  try {
    await throwThroughFinally();
    $ERROR("Expected the original error");
  } catch (error) {
    assert.sameValue(error, original);
  }
  assert.compareArray(events, ["finally"]);

  var sameMicrotask = true;
  var disposedInSameMicrotask;
  async function mixedResources() {
    using sync = {
      [Symbol.dispose]() { disposedInSameMicrotask = sameMicrotask; }
    };
    await using first = null, second = undefined;
  }
  var completion = mixedResources();
  sameMicrotask = false;
  await completion;
  assert.sameValue(disposedInSameMicrotask, false);

  events = [];
  var previous = Object.getOwnPropertyDescriptor(Function.prototype, Symbol.dispose);
  try {
    Function.prototype[Symbol.dispose] = function () { events.push(this.name); };
    function callableResources() {
      using unused = function () {};
      using directOnly = () => {};
      directOnly();
    }
    callableResources();
    assert.compareArray(events, ["directOnly", "unused"]);
  } finally {
    if (previous) Object.defineProperty(Function.prototype, Symbol.dispose, previous);
    else delete Function.prototype[Symbol.dispose];
  }

  events = [];
  var assignmentError = new Error("setter");
  var target = { set value(value) { throw assignmentError; } };
  try {
    await using value = resource("setter-cleanup");
    target.value = 1;
  } catch (error) {
    assert.sameValue(error, assignmentError);
  }
  assert.compareArray(events, ["setter-cleanup"]);

  events = [];
  var getterError = new Error("getter");
  target = { get value() { throw getterError; } };
  try {
    await using value = resource("getter-cleanup");
    (await Promise.resolve(target)).value;
  } catch (error) {
    assert.sameValue(error, getterError);
  }
  assert.compareArray(events, ["getter-cleanup"]);

  events = [];
  try {
    await using value = resource("conditional-cleanup");
    (false ? await Promise.resolve(null) : await Promise.resolve(target)).value;
  } catch (error) {
    assert.sameValue(error, getterError);
  }
  assert.compareArray(events, ["conditional-cleanup"]);

  events = [];
  function* generator() {
    using value = { [Symbol.dispose]() { events.push("generator-cleanup"); } };
    try { (yield target).value; }
    catch (error) { assert.sameValue(error, getterError); }
  }
  var iterator = generator();
  assert.sameValue(iterator.next().value, target);
  assert.sameValue(iterator.next(target).done, true);
  assert.compareArray(events, ["generator-cleanup"]);

  events = [];
  switch (1) {
    case 1: {
      await using firstCase = resource("first-case");
    }
    case 2: {
      await using secondCase = resource("second-case");
      break;
    }
  }
  events.push("after-switch");
  assert.compareArray(events, ["first-case", "second-case", "after-switch"]);
});
