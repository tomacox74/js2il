public class ModuleCacheEntry
{
    public required string Id { get; set; }
}

public class ModuleCache
{
    private readonly Dictionary<string, ModuleCacheEntry> _moduleCache = new ();

    public bool TryGetModule(string path, out ModuleCacheEntry? module)
    {
        return _moduleCache.TryGetValue(path, out module);
    }

    public void AddModule(string path, ModuleCacheEntry module)
    {
        _moduleCache[path] = module;
    }
}