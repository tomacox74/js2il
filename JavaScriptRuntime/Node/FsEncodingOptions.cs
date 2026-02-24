using System;
using System.Text;

namespace JavaScriptRuntime.Node
{
    internal static class FsEncodingOptions
    {
        internal static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        internal static bool TryGetTextEncoding(object? options, out Encoding? encoding)
        {
            encoding = null;
            if (options == null || options is JsNull)
            {
                return false;
            }

            if (options is string optionString)
            {
                return TryResolveTextEncoding(optionString, out encoding);
            }

            try
            {
                var encodingValue = JavaScriptRuntime.ObjectRuntime.GetProperty(options, "encoding");
                if (encodingValue == null || encodingValue is JsNull)
                {
                    return false;
                }

                return TryResolveTextEncoding(encodingValue.ToString() ?? string.Empty, out encoding);
            }
            catch
            {
                return false;
            }
        }

        private static bool TryResolveTextEncoding(string value, out Encoding? encoding)
        {
            encoding = null;
            if (value.Equals("utf8", StringComparison.OrdinalIgnoreCase)
                || value.Equals("utf-8", StringComparison.OrdinalIgnoreCase))
            {
                encoding = Utf8NoBom;
                return true;
            }

            return false;
        }
    }
}