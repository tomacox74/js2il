namespace JavaScriptRuntime.EngineCore;

internal interface ICleanupJobScheduler
{
    void QueueCleanupJob(Action task);
}
