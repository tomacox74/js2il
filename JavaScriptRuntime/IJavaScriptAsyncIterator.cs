namespace JavaScriptRuntime;

/// <summary>
/// Async iterator interface used by for await..of lowering.
///
/// Next/Return return a JavaScript value that may be a Promise (or a plain value).
/// The compiler always awaits these results.
/// </summary>
/// <remarks>
/// ECMA-262 references:
/// - 27.1.1.4 The Async Iterator Interface: https://tc39.es/ecma262/#sec-asynciterator-interface
/// - 27.1.1.3 The Async Iterable Interface (async-iterables expose @@asyncIterator): https://tc39.es/ecma262/#sec-asynciterable-interface
/// - 20.4.2.1 Symbol.asyncIterator (well-known symbol): https://tc39.es/ecma262/#sec-symbol.asynciterator
/// - 7.4.14 AsyncIteratorClose ( iteratorRecord , completion ): https://tc39.es/ecma262/#sec-asynciteratorclose
/// - 7.4.15 IfAbruptCloseAsyncIterator ( value , iteratorRecord ): https://tc39.es/ecma262/#sec-ifabruptcloseasynciterator
/// </remarks>
public interface IJavaScriptAsyncIterator
{
    object? Next();

    bool HasReturn { get; }

    object? Return();
}
