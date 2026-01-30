"use strict";\r\n\r\nconst show = ({a, b}) => {
  console.log(a, b);
};

show({ a: 1, b: 2 });

// Test with default parameters
const config = ({host = "localhost", port = 8080, secure = false}) => {
  console.log((secure ? "https://" : "http://") + host + ":" + port);
};

// Test with some defaults used
config({ host: "example.com" });

// Test with all values provided
config({ host: "api.test.com", port: 3000, secure: true });

// Test with all defaults used
config({});
