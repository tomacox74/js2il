using Mel = Microsoft.Extensions.Logging;

namespace Js2IL.Diagnostics;

internal sealed class TextWriterLoggerProvider : Mel.ILoggerProvider
{
    private readonly TextWriter _writer;
    private readonly bool _ownsWriter;
    private readonly object _sync = new();

    public TextWriterLoggerProvider(TextWriter writer, bool ownsWriter)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _ownsWriter = ownsWriter;
    }

    public Mel.ILogger CreateLogger(string categoryName)
    {
        return new TextWriterLogger(categoryName, this);
    }

    public void Dispose()
    {
        if (_ownsWriter)
        {
            _writer.Dispose();
        }
    }

    private void Write(Mel.LogLevel level, string categoryName, string message, Exception? exception)
    {
        lock (_sync)
        {
            _writer.Write('[');
            _writer.Write(level);
            _writer.Write("] ");
            _writer.Write(categoryName);
            _writer.Write(": ");
            _writer.WriteLine(message);

            if (exception != null)
            {
                _writer.WriteLine(exception);
            }

            _writer.Flush();
        }
    }

    private sealed class TextWriterLogger : Mel.ILogger
    {
        private readonly string _categoryName;
        private readonly TextWriterLoggerProvider _provider;

        public TextWriterLogger(string categoryName, TextWriterLoggerProvider provider)
        {
            _categoryName = categoryName;
            _provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return NoopScope.Instance;
        }

        public bool IsEnabled(Mel.LogLevel logLevel)
        {
            return logLevel != Mel.LogLevel.None;
        }

        public void Log<TState>(
            Mel.LogLevel logLevel,
            Mel.EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception == null)
            {
                return;
            }

            _provider.Write(logLevel, _categoryName, message, exception);
        }
    }

    private sealed class NoopScope : IDisposable
    {
        public static readonly NoopScope Instance = new();

        public void Dispose()
        {
        }
    }
}
