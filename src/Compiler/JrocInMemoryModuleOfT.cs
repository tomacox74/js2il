namespace Jroc;

public sealed class JrocInMemoryModule<TExports> : JrocInMemoryModule
    where TExports : class
{
    internal JrocInMemoryModule(JrocLoadedAssembly loadedAssembly, TExports exports)
        : base(
            loadedAssembly,
            exports,
            exports as IDisposable
                ?? throw new InvalidOperationException($"{typeof(TExports).FullName} must implement IDisposable."))
    {
    }

    public new TExports Exports => (TExports)base.Exports;
}
