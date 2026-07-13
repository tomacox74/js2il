using JavaScriptRuntime;
using RuntimeError = JavaScriptRuntime.Error;
using RuntimeTypeError = JavaScriptRuntime.TypeError;
using RuntimeHttpAgent = JavaScriptRuntime.Node.HttpAgent;
using RuntimeHttpsModule = JavaScriptRuntime.Node.Https;
using RuntimeTlsModule = JavaScriptRuntime.Node.Tls;

namespace Jroc.Tests.Node.Https
{
    public class RuntimeTests
    {
        [Fact]
        public void Tls_CreateSecureContext_KeyWithoutCert_ThrowsClearError()
        {
            var options = new JsObject();
            options["key"] = "-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----";

            var tls = new RuntimeTlsModule();
            var ex = Assert.Throws<RuntimeError>(() => tls.createSecureContext(options));
            Assert.Contains("both key and cert", ex.Message);
        }

        [Fact]
        public void Https_Request_WithAgentObject_ThrowsClearError()
        {
            var options = new JsObject();
            options["host"] = "127.0.0.1";
            options["port"] = 443.0;
            options["agent"] = new RuntimeHttpAgent();

            var https = new RuntimeHttpsModule();
            var ex = Assert.Throws<RuntimeTypeError>(() => https.request(new object[] { options }));
            Assert.Contains("agent=false", ex.Message);
        }
    }
}
