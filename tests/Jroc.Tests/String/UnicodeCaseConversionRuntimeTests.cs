using System.Globalization;
using JavaScriptRuntime;

namespace Jroc.Tests.String;

public sealed class UnicodeCaseConversionRuntimeTests
{
    [Theory]
    [InlineData("")]
    [InlineData("en-US")]
    [InlineData("tr-TR")]
    [InlineData("az-Latn-AZ")]
    public void AsciiFastPathPreservesCultureCasing(string cultureName)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        var ascii = new string(Enumerable.Range(0, 128).Select(value => (char)value).ToArray());

        foreach (var length in new[] { 0, 1, 7, 15, 16, 17, 31, 32, 33, 127, 128, 129, 4096 })
        {
            var input = string.Concat(Enumerable.Repeat(ascii, length / ascii.Length + 1))[..length];
            Assert.Equal(input.ToLower(culture), UnicodeCaseConversion.ToLower(input, culture));
            Assert.Equal(input.ToUpper(culture), UnicodeCaseConversion.ToUpper(input, culture));
        }

        Assert.Equal("I".ToLower(culture), UnicodeCaseConversion.ToLower("I", culture));
        Assert.Equal("i".ToUpper(culture), UnicodeCaseConversion.ToUpper("i", culture));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(4096)]
    public void NonAsciiMappingsRemainActiveAfterAsciiPrefix(int prefixLength)
    {
        var prefix = new string('a', prefixLength);
        Assert.Equal(prefix + "i\u0307", JavaScriptRuntime.String.ToLowerCase(prefix + "\u0130"));
        Assert.Equal(prefix.ToUpperInvariant() + "SS", JavaScriptRuntime.String.ToUpperCase(prefix + "\u00DF"));
        Assert.Equal(prefix + "\u03BF\u03C2", JavaScriptRuntime.String.ToLowerCase(prefix + "\u039F\u03A3"));
        Assert.Equal(prefix + "\u03BF\u03C3a", JavaScriptRuntime.String.ToLowerCase(prefix + "\u039F\u03A3A"));
        Assert.Equal(prefix + "\u1C8A", JavaScriptRuntime.String.ToLowerCase(prefix + "\u1C89"));
        Assert.Equal(prefix + "\U00010D70", JavaScriptRuntime.String.ToLowerCase(prefix + "\U00010D50"));
        Assert.Equal(prefix.ToUpperInvariant() + "\U00010D50",
            JavaScriptRuntime.String.ToUpperCase(prefix + "\U00010D70"));
        Assert.Equal(prefix + "\uD800", JavaScriptRuntime.String.ToLowerCase(prefix + "\uD800"));
        Assert.Equal(prefix.ToUpperInvariant() + "\uDC00",
            JavaScriptRuntime.String.ToUpperCase(prefix + "\uDC00"));
    }
}
