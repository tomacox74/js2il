namespace JavaScriptRuntime.EngineCore;

interface IScheduler
{
    object Schedule(Action action, TimeSpan delay);
    void Cancel(object handle);

    object ScheduleInterval(Action action, TimeSpan interval);
    void CancelInterval(object handle);

    object ScheduleImmediate(Action action);
    void CancelImmediate(object handle);
}