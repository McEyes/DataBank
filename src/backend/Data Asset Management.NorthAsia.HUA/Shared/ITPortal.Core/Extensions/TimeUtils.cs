using System;
using System.Diagnostics;

namespace ITPortal.Core;

public static class TimeUtils
{
    /// <summary>
    /// Microsecond|微秒
    /// milliseconds
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static double TestMicrosecond(Action action)
    {
        var sw = new Stopwatch();
        sw.Start();
        action();
        sw.Stop();
        // 计算纳秒
        long elapsedTicks = sw.ElapsedTicks;
        double nanosecondsPerTick = (1_000_000_000.0) / Stopwatch.Frequency;
        double elapsedNanoseconds = elapsedTicks * nanosecondsPerTick / 1000;
        return elapsedNanoseconds;
    }

    /// <summary>
    /// Microsecond|微秒
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<double> TestMicrosecondAsync(Func<Task> asyncAction)
    {
        var sw = new Stopwatch();
        sw.Start();
        await asyncAction();
        sw.Stop();
        // 计算纳秒
        long elapsedTicks = sw.ElapsedTicks;
        double nanosecondsPerTick = (1_000_000_000.0) / Stopwatch.Frequency;
        double elapsedNanoseconds = elapsedTicks * nanosecondsPerTick / 1000;
        return elapsedNanoseconds;
    }
}