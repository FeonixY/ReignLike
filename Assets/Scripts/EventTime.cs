using System;

/// <summary>
/// 事件发生的时间阶段，支持位运算组合。
/// </summary>
[Flags]
public enum EventTime
{
    None = 0,
    Early = 1 << 0,
    Middle = 1 << 1,
    Late = 1 << 2,
    All = Early | Middle | Late
}
