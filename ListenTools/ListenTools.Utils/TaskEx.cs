namespace ListenTools.Utils;

public static class TaskEx
{ 
    /// <summary>
    /// 延迟执行任务
    /// </summary>
    /// <param name="dueTime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Task Delay(int dueTime)
    {
        if (dueTime < -1) throw new ArgumentOutOfRangeException("dueTime");

        Timer? timer = null;
        var source = new TaskCompletionSource<bool>();
        timer = new Timer(_ =>
        {
            using (timer) source.TrySetResult(true);
        },null,dueTime,Timeout.Infinite);

        return source.Task;
    }
}