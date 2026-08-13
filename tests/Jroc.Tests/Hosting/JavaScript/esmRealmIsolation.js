export let value = 0;

export function increment() {
  value++;
  return value;
}

export function read() {
  return value;
}
