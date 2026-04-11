"use strict";

const tls = require('node:tls');
const { certPem, keyPem } = require('./Tls_TestCertificates');

try {
  tls.createServer({ key: keyPem, cert: certPem, requestCert: true });
  console.log('unexpected success');
} catch (error) {
  console.log('message:' + error.message);
}
