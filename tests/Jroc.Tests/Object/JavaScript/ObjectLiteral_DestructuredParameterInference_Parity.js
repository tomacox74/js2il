let config = {
  sieveSize: 1000000,
  timeLimitSeconds: 5,
  verbose: false,
  runtime: ""
};

config.runtime = process.argv[0];
config.verbose = process.argv.includes("verbose");

const main = ({ sieveSize, timeLimitSeconds, verbose, runtime }) => {
  console.log(sieveSize + timeLimitSeconds);
  console.log(typeof sieveSize);
  console.log(typeof timeLimitSeconds);
  console.log(typeof verbose);
  console.log(typeof runtime);
};

main(config);
