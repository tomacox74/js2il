var descriptor = Object.getOwnPropertyDescriptor(Map.prototype, 'size');

verifyProperty(descriptor.get, "length", {
  value: 0,
  writable: false,
  enumerable: false,
  configurable: true
});

