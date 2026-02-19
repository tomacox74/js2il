"use strict";

const events = require("node:events");
const EventEmitter = events.EventEmitter;

async function runOnBreak() {
  const emitter = new EventEmitter();
  const iterator = events.on(emitter, "tick");

  emitter.emit("tick", 1, 2);
  emitter.emit("tick", 3);

  let count = 0;
  for await (const args of iterator) {
    console.log(args.length);
    console.log(args[0]);
    count++;
    if (count === 2) {
      break;
    }
  }

  emitter.emit("tick", 99);
  console.log("break-done");
}

async function runOnErrorReject() {
  const emitter = new EventEmitter();
  const iterator = events.on(emitter, "data");

  const loop = (async () => {
    try {
      for await (const args of iterator) {
        console.log(args[0]);
      }
      console.log("NO_REJECT");
    } catch (e) {
      console.log("iter-reject");
    }
  })();

  emitter.emit("error", "E1");
  await loop;
}

async function runOnceResolve() {
  const emitter = new EventEmitter();
  const p = events.once(emitter, "ready");
  emitter.emit("ready", "ok", 42);

  const args = await p;
  console.log(args.length);
  console.log(args[0]);
  console.log(args[1]);
}

async function runOnceReject() {
  const emitter = new EventEmitter();
  const p = events.once(emitter, "ready");
  emitter.emit("error", "boom");

  try {
    await p;
    console.log("NO_REJECT");
  } catch (e) {
    console.log("once-reject");
  }
}

runOnBreak()
  .then(runOnErrorReject)
  .then(runOnceResolve)
  .then(runOnceReject)
  .then(() => {
    console.log("done");
  });
