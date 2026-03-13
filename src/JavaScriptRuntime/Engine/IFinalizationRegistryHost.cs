namespace JavaScriptRuntime.EngineCore;

public interface IFinalizationRegistryHost
{
    void TrackRegistry(JavaScriptRuntime.FinalizationRegistry registry);
    void AddToKeptObjects(object target);
    void ClearKeptObjects();
    void CollectAndQueueCleanupJobs(bool forceCollection);
}
