namespace JavaScriptRuntime.EngineCore;

interface IMicrotaskScheduler
{
    void QueueMicrotask(Action task);
}